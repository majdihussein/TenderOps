using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using AutoMapper.Internal;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using static TenderOps_Utility.SD;

namespace TenderOps_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        private readonly IApiMessageRequestBuilder _apimessageRequestBuilder;
        protected readonly string tenderUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider
            , IConfiguration configuration, IHttpContextAccessor httpContextAccessor
            , IApiMessageRequestBuilder apimessageRequestBuilder)
        {
            _tokenProvider = tokenProvider;
            this.responseModel = new();
            tenderUrl = configuration.GetValue<string>("ServiceUrls:API");
            this.httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _apimessageRequestBuilder = apimessageRequestBuilder;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest, bool withBearer = true)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");

                var messageFactory = () =>
                {
                    return _apimessageRequestBuilder.Build(apiRequest);
                };

                HttpResponseMessage httpResponseMessage = await SendWithRefreshTokenAsync(client, messageFactory, withBearer);

                APIResponse FinalApiResponse = new()
                {
                    IsSuccess = false
                };

                try
                {
                    switch (httpResponseMessage.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            FinalApiResponse.ErrorMessage = new List<string> { "Not Found" };
                            break;
                        case HttpStatusCode.Forbidden:
                            FinalApiResponse.ErrorMessage = new List<string> { "Access Denied" };
                            break;
                        case HttpStatusCode.Unauthorized:
                            FinalApiResponse.ErrorMessage = new List<string> { "Unauthorized" };
                            break;
                        case HttpStatusCode.InternalServerError:
                            FinalApiResponse.ErrorMessage = new List<string> { "Internal Server Error" };
                            break;
                        default:
                            var apiContent = await httpResponseMessage.Content.ReadAsStringAsync();
                            FinalApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent) ?? new APIResponse
                            {
                                IsSuccess = false,
                                ErrorMessage = new List<string> { "Empty response from API." }
                            };
                            break;
                    }
                }
                catch (Exception e)
                {
                    FinalApiResponse.ErrorMessage = new List<string> { "Error Encountered", e.Message };
                }

                var serializedResponse = JsonConvert.SerializeObject(FinalApiResponse);

                // إذا النوع T هو قائمة أو نوع عام
                if (typeof(T) == typeof(List<TenderCategoryDTO>) || typeof(T) == typeof(List<TenderDTO>) || typeof(T).IsGenericType)
                {
                    var apiResponse = JsonConvert.DeserializeObject<APIResponse>(serializedResponse);

                    if (apiResponse?.Result == null)
                    {
                        return default(T);
                    }

                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(apiResponse.Result));
                }

                // الحالة العادية: T هو APIResponse أو كائن بسيط
                return JsonConvert.DeserializeObject<T>(serializedResponse);
            }
            catch (AuthException)
            {
                throw;
            }
            catch (Exception e)
            {
                var errorResponse = new APIResponse
                {
                    ErrorMessage = new List<string> { e.Message },
                    IsSuccess = false
                };
                var serializedError = JsonConvert.SerializeObject(errorResponse);
                return JsonConvert.DeserializeObject<T>(serializedError);
            }
        }


        // ارساله مع اكسز توكين وتجديد التوكين اذا لزم
        private async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient httpClient,
            Func<HttpRequestMessage> httpRequestMessageFactory, bool withBearer = true)
        {
            if (!withBearer)
            {
                return await httpClient.SendAsync(httpRequestMessageFactory());
            }
            else
            {
                TokenDTO tokenDTO = _tokenProvider.GetToken();
                if(tokenDTO != null && !string.IsNullOrEmpty(tokenDTO.AccessToken))
                {
                     httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDTO.AccessToken);
                }

                try
                {
                    var response = await httpClient.SendAsync(httpRequestMessageFactory());
                    if(response.IsSuccessStatusCode)
                        return response;

                     // If the fails the can pass refresh token!
                     if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
                     {
                         //Genarate New Token from token / sign in with new token and then entry
                         await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                         response = await httpClient.SendAsync(httpRequestMessageFactory());
                         return response;
                     }
                    return response;

                }
                catch (AuthException)
                {
                    throw;
                }
                catch (HttpRequestException httpRequestException)
                {
                    if (httpRequestException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // refresh token and retry request
                        await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
                        return await httpClient.SendAsync(httpRequestMessageFactory());
                    }
                    throw;
                }
            }
        }

        // طلب تجديد التوكين
        private async Task InvokeRefreshTokenEndpoint(HttpClient httpClient, string existingAccessToken, string existingRefreshToken)
        {
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri($"{tenderUrl}/api/{SD.CurrentAPIVersion}/UsersAuth/refresh");
            message.Method = HttpMethod.Post;
            message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDTO()
            {
                AccessToken = existingAccessToken,
                RefreshToken = existingRefreshToken
            }), Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                // في حال استجابة غير ناجحة
                await _httpContextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw new AuthException();
            }

            var content = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);

            //if (apiResponse?.IsSuccess != true )
            if(apiResponse == null || !apiResponse.IsSuccess || apiResponse.Result == null)
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                throw new AuthException();
            }
            else
            {
                var tokenDataStr = JsonConvert.SerializeObject(apiResponse.Result);
                var tokenDto = JsonConvert.DeserializeObject<TokenDTO>(tokenDataStr);

                if ( tokenDto != null && !string.IsNullOrEmpty(tokenDto.AccessToken))
                {
                    // New method to sign in with new token that we receive
                    await SignInWithNewTokens(tokenDto);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
                }
            }
        }

        // عملية التسجيل بالتوكين الجديد وانشاء هوية جديدة للمستخدم 
        private async Task SignInWithNewTokens(TokenDTO tokenDTO)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);


            // انشاء هوية كوكي للمستخدم للتنقل دون الحاجة للدخول مرات عديدة في كل حركة
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme); // انشاء هوية جديدة

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt
                .Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt
                .Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity); // انشاء كائن جديد من ClaimsPrincipal
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            _tokenProvider.SetToken(tokenDTO); // تخزين التوكين في الجلسة
        }
    }
}

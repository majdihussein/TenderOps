using System.Text;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Services.IServices;
using Newtonsoft.Json;
using static TenderOps_Utility.SD;

namespace TenderOps_Web.Services
{
    public class ApiMessageRequestBuilder : IApiMessageRequestBuilder
    {
        public HttpRequestMessage Build(APIRequest apiRequest)
        {
            HttpRequestMessage message = new();

            if (apiRequest.ContentType == ContentType.MultipartFormData) // for muliparttype image
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }

            message.RequestUri = new Uri(apiRequest.Url); // for api url

            if (apiRequest.ContentType == ContentType.MultipartFormData) // for multipart image/form-data
            {
                var content = new MultipartFormDataContent(); // اذا لقيتها حولها للصيغة هاي

                foreach (var prop in apiRequest.Data.GetType().GetProperties()) // لف ع كل api لانو هاد عام
                {
                    var value = prop.GetValue(apiRequest.Data); // ثبت القيمة
                    if (value is IFormFile)
                    {
                        var file = (IFormFile)value; // اذا القيمة هي ملف
                        if (file != null)
                        {
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        // لمعالجة اي طلب مستقبلي بخواص اخرى
                    }
                }
                message.Content = content; // حط المحتوى في الرسالة
            }
            else
            {
                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
            }

            switch (apiRequest.ApiType)
            {
                case SD.ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }
            return message;
        }
    }
}

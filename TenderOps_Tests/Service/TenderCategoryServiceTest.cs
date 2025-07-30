using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services;
using TenderOps_Web.Services.IServices;
using static TenderOps_Utility.SD;

    public class TenderCategoryServiceTest
    {
        private readonly Mock<IHttpClientFactory> mockClientFactory;
        private readonly Mock<IBaseService> mockBaseService;
        private readonly IConfiguration configuration;

        public TenderCategoryServiceTest()
        {
            mockClientFactory = new Mock<IHttpClientFactory>();
            mockBaseService = new Mock<IBaseService>();
            var inMeoerySettings = new Dictionary<string, string>
            {
                {"ServiceUrls:API", "https://localhost:5001"}
            };
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMeoerySettings)
                .Build();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess()
        {
            // Arrange
            var tenderCategoryCreateDTO = new TenderCategoryCreateDTO
            {
                Name = "Test Category"
            };
            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                IsSuccess = true,
                ErrorMessage = null,
                Result = "Category created successfully"
            };

            mockBaseService.SetupProperty( u =>u.responseModel);
            mockBaseService.Setup(u => u.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var tenderCategoryservice = new TenderCategoryService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await tenderCategoryservice.CreateAsync<APIResponse>(tenderCategoryCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("Category created successfully", result.Result);
            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(r =>
            r.ApiType == SD.ApiType.POST &&
            r.Data == tenderCategoryCreateDTO &&
            r.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderCategoryAPI"
            ), true), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccess()
        {
            // Arrange
            int categoryId = 1;

            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.NoContent,
                IsSuccess = true,
                ErrorMessage = null,
                Result = null
            };

            mockBaseService.SetupProperty(u => u.responseModel);
            mockBaseService.Setup(u => u.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var tenderCategoryservice = new TenderCategoryService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await tenderCategoryservice.DeleteAsync<APIResponse>(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(r =>
            r.ApiType == SD.ApiType.DELETE &&
            r.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderCategoryAPI/{categoryId}"
            ), true), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSuccess()
        {
            // Arrange

            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.NoContent,
                IsSuccess = true,
                ErrorMessage = null,
                Result = null
            };

            mockBaseService.SetupProperty(u => u.responseModel);
            mockBaseService.Setup(u => u.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var tenderCategoryservice = new TenderCategoryService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await tenderCategoryservice.GetAllAsync<APIResponse>();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(r =>
            r.ApiType == SD.ApiType.GET &&
            r.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderCategoryAPI?pagesize=0"
            ), true), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnSuccess()
        {
            // Arrange
            int categoryId = 1;
            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.NoContent,
                IsSuccess = true,
                ErrorMessage = null,
                Result = null
            };

            mockBaseService.SetupProperty(u => u.responseModel);
            mockBaseService.Setup(u => u.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var tenderCategoryservice = new TenderCategoryService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await tenderCategoryservice.GetAsync<APIResponse>(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(r =>
            r.ApiType == SD.ApiType.GET &&
            r.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderCategoryAPI/{categoryId}"
            ), true), Times.Once);
        }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess()
    {
        //Arrange
       var mockClientFactory = new Mock<IHttpClientFactory>();
        var mockBaseService = new Mock<IBaseService>();

        var inMemorySettings = new Dictionary<string, string> {
        {"ServiceUrls:API", "##"} };
        TenderCategoryUpdateDTO tenderCategoryUpdateDTO = new TenderCategoryUpdateDTO
        {
            Id = 1,
            Name = "Updated Category"
        };


        var expectedResponse = new APIResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            ErrorMessage = null,
            Result = "Tender Updated"
        };

        mockBaseService.SetupProperty(s => s.responseModel);
        mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

        var tenderCategoryService = new TenderCategoryService(mockClientFactory.Object, configuration, mockBaseService.Object);

        //act
       var result = await tenderCategoryService.UpdateAsync<APIResponse>(tenderCategoryUpdateDTO);

        //assert
            Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Tender Updated", result.Result);
        mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
            req.ApiType == SD.ApiType.PUT &&
            req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderCategoryAPI/{tenderCategoryUpdateDTO.Id}" &&
            req.Data == tenderCategoryUpdateDTO
        ), true), Times.Once);

    }



}

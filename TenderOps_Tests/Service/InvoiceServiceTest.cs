using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services;
using TenderOps_Web.Services.IServices;
using static TenderOps_Utility.SD;

namespace TenderOps_Tests.Service
{
    public class InvoiceServiceTest
    {
        private readonly Mock<IHttpClientFactory> mockClientFactory;
        private readonly Mock<IBaseService> mockBaseService;
        private readonly IConfiguration configuration;

        public InvoiceServiceTest()
        {
            mockClientFactory = new Mock<IHttpClientFactory>();
            mockBaseService = new Mock<IBaseService>();

            var inMemorySettings = new Dictionary<string, string> {
            { "ServiceUrls:API", "https://localhost:5001" }
        };

            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccessfulAPIResponse()
        {
            // Arrange
            var invoiceCreateDTO = new InvoiceCreateDTO
            {
                InvoiceNumber = "RTEN10",
                Amount = 55,
                Notes = "A major road construction project in downtown.",
                TenderId = 2,
                CreatedBy = "Open",
                UserId = null
            };

            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                IsSuccess = true,
                ErrorMessage = null,
                Result = "Invoice Created"
            };

            mockBaseService.SetupProperty(s => s.responseModel);
            mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var invoiceService = new InvoiceService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await invoiceService.CreateAsync<APIResponse>(invoiceCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal("Invoice Created", result.Result);

            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
                req.ApiType == SD.ApiType.POST &&
                req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/InvoiceAPI/" &&
                req.Data == invoiceCreateDTO
            ), true), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnSuccessfulAPIResponse()
        {
            // Arrange
            int id = 1;
            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.NoContent,
                IsSuccess = true,
                ErrorMessage = null,
                Result = "Invoice Deleted"
            };

            mockBaseService.SetupProperty(s => s.responseModel);
            mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var invoiceService = new InvoiceService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await invoiceService.DeleteAsync<APIResponse>(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            Assert.Equal("Invoice Deleted", result.Result);

            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
                req.ApiType == SD.ApiType.DELETE &&
                req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/InvoiceAPI/{id}"
            ), true), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSuccessfulAPIResponse()
        {
            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                ErrorMessage = null,
                Result = null
            };

            mockBaseService.SetupProperty(s => s.responseModel);
            mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var invoiceService = new InvoiceService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await invoiceService.GetAllAsync<APIResponse>();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
                req.ApiType == SD.ApiType.GET &&
                req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/InvoiceAPI"
            ), true), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnSuccessfulAPIResponse()
        {
            int id = 1;
            var expectedResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                ErrorMessage = null,
                Result = "Invoice View"
            };

            mockBaseService.SetupProperty(s => s.responseModel);
            mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

            var invoiceService = new InvoiceService(mockClientFactory.Object, configuration, mockBaseService.Object);

            // Act
            var result = await invoiceService.GetAsync<APIResponse>(id);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Invoice View", result.Result);

            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
                req.ApiType == SD.ApiType.GET &&
                req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/InvoiceAPI/{id}"
            ), true), Times.Once);

        }

    }
}

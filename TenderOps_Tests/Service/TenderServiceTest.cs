using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services;
using TenderOps_Web.Services.IServices;
using Xunit;
using static TenderOps_Utility.SD;

public class TenderServiceTests
{
    private readonly Mock<IHttpClientFactory> mockClientFactory;
    private readonly Mock<IBaseService> mockBaseService;
    private readonly IConfiguration configuration;

    public TenderServiceTests()
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
    public async Task CreateAsync_ShouldReturnSuccessfulAPIResponse()
    {
        /// Arrange
        // tendercreatedto
        // expected response
        // baseservice
        var tenderCreateDTO = new TenderCreateDTO
        {
            TenderName = "Road Construction Project",
            TenderNumber = "TND-2025-001",
            Description = "A major road construction project in downtown.",
            Location = TenderLocation.Ramallah,
            Status = "Open",
            ImageUrl = null,
            Image = null,
            TenderStartDate = DateTime.UtcNow,
            TenderEndDate = DateTime.UtcNow.AddDays(30),
            CreatedByUserId = "user-123",
            TenderCategoryId = 1,
            PartnerId = 10
        };

        var expectedResponse = new APIResponse
        {
            StatusCode = HttpStatusCode.Created,
            IsSuccess = true,
            ErrorMessage = null,
            Result = "Tender Created"
        };

        mockBaseService.SetupProperty(s => s.responseModel);
        mockBaseService.Setup(s =>s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
            .ReturnsAsync(expectedResponse);

        var tenderService = new TenderService(mockClientFactory.Object, configuration, mockBaseService.Object);

        // Act
        var result = await tenderService.CreateAsync<APIResponse>(tenderCreateDTO);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Equal("Tender Created", result.Result);
        Assert.True(result.IsSuccess);
            mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
            req.ApiType == SD.ApiType.POST &&
            req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderAPI" &&
            req.ContentType == SD.ContentType.MultipartFormData &&
            req.Data == tenderCreateDTO
        ), true), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccessfulAPIResponse()
    {
        int tenderId = 123;

        var expectedResponse = new APIResponse
        {
            StatusCode = HttpStatusCode.NoContent,
            IsSuccess = true,
            ErrorMessage = null,
            Result = null
        };

        mockBaseService.SetupProperty(s => s.responseModel);
        mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

        var tenderService = new TenderService(mockClientFactory.Object, configuration, mockBaseService.Object);

        // act
        var result = await tenderService.DeleteAsync<APIResponse>(tenderId);

        // assert
        Assert.NotNull(result);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

        mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
            req.ApiType == SD.ApiType.DELETE &&
            req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderAPI/{tenderId}"
        ), true), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSuccessfulAPIResponse()
    {
        var expectedResponse = new APIResponse
        {
            StatusCode = HttpStatusCode.NoContent,
            IsSuccess = true,
            ErrorMessage = null,
            Result = null
        };

        mockBaseService.SetupProperty(s => s.responseModel);
        mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

        var tenderService = new TenderService(mockClientFactory.Object, configuration, mockBaseService.Object);

        // act
        var result = await tenderService.GetAllAsync<APIResponse>();

        // assert
        Assert.NotNull(result);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);

        mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
            req.ApiType == SD.ApiType.GET &&
            req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderAPI?pagesize=0"
        ), true), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnSuccessfulAPIResponse()
    {
        int tenderId = 444;

        var expectedResponse = new APIResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            ErrorMessage = null,
            Result = "Tender Details"
        };

        mockBaseService.SetupProperty(s => s.responseModel);
        mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
                .ReturnsAsync(expectedResponse);

        var tenderService = new TenderService(mockClientFactory.Object, configuration, mockBaseService.Object);

        // act
        var result = await tenderService.GetAsync<APIResponse>(tenderId);

        // assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Tender Details", result.Result);
        mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
            req.ApiType == SD.ApiType.GET &&
            req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderAPI/{tenderId}"
        ), true), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccessfulAPIResponse()
    {
        TenderUpdateDTO tenderUpdateDTO = new TenderUpdateDTO
        {
            TenderName = "Road Construction Project",
            TenderNumber = "TND-2025-001",
            Description = "A major road construction project in downtown.",
            Location = TenderLocation.Ramallah,
            Status = "Open",
            ImageUrl = null,
            Image = null,
            TenderStartDate = DateTime.UtcNow,
            TenderEndDate = DateTime.UtcNow.AddDays(30),
            TenderCategoryId = 1,
            PartnerId = 10
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

        var tenderService = new TenderService(mockClientFactory.Object, configuration, mockBaseService.Object);

        // act
        var result = await tenderService.UpdateAsync<APIResponse>(tenderUpdateDTO);

        // assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Tender Updated", result.Result);
        mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
            req.ApiType == SD.ApiType.PUT &&
            req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderAPI/{tenderUpdateDTO.Id}" && 
            req.ContentType == SD.ContentType.MultipartFormData &&
            req.Data == tenderUpdateDTO
        ), true), Times.Once);
    }

    [Fact]
    public async Task GetTenderPartner()
    {

        // Arrange
        var mockClientFactory = new Mock<IHttpClientFactory>();
        var mockBaseService = new Mock<IBaseService>();

        var inMemorySettings = new Dictionary<string, string> {
        {"ServiceUrls:API", "https://localhost:5001"}
    };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        int partnerId = 321;

        var expectedResponse = new APIResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            ErrorMessage = null,
            Result = "Partner Tender Info"
        };

        mockBaseService.SetupProperty(s => s.responseModel);
        mockBaseService.Setup(s => s.SendAsync<APIResponse>(It.IsAny<APIRequest>(), true))
            .ReturnsAsync(expectedResponse);

        var tenderService = new TenderService(mockClientFactory.Object, configuration, mockBaseService.Object);

        // Act
        var result = await tenderService.GetTenderPartner<APIResponse>(partnerId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Partner Tender Info", result.Result);
        mockBaseService.Verify(s => s.SendAsync<APIResponse>(It.Is<APIRequest>(req =>
            req.ApiType == SD.ApiType.GET &&
            req.Url == $"https://localhost:5001/api/{SD.CurrentAPIVersion}/TenderAPI/GetTenderPartner/{partnerId}"
            ), true), Times.Once);
    }
}

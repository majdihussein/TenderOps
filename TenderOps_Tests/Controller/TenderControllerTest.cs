using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Newtonsoft.Json;
using TenderOps_Web.Controllers;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;
using static TenderOps_Utility.SD;

namespace TenderOps_Tests.Controller
{
    public class TenderControllerTest
    {
        [Fact]
        public async Task IndexTender_ReturnResult()
        {
            // Arrange
            var tenderlist = new List<TenderDTO>
            {
                new TenderDTO
                {
                     Id = 1,
                     TenderName = "Test Tender",
                     TenderNumber = "TN-001",
                     Description = "Test Description",
                     Location = TenderLocation.Ramallah,
                     Status = "Open",
                     TenderStartDate = DateTime.Now,
                     TenderEndDate = DateTime.Now.AddDays(10),
                     TenderCategoryId = 1,
                     PartnerId = 1
                }
            };

            var apiResponse = new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = JsonConvert.SerializeObject(tenderlist)
            };
            var mockTenderService = new Mock<ITenderService>();
            mockTenderService
                .Setup(y => y.GetAllAsync<APIResponse>())
                .ReturnsAsync(apiResponse);

            // Act
            var controller = new TenderController(
                mockTenderService.Object,
                Mock.Of<IMapper>(),
                Mock.Of<ITenderCategoryService>(),
                Mock.Of<IPartnerService>());

            var result = await controller.IndexTender();

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult.Model);
            Assert.IsType<List<TenderDTO>>(viewResult.Model);
            var model = viewResult.Model as List<TenderDTO>;
            Assert.Single(model);
            Assert.Equal("Test Tender", model[0].TenderName);
            Assert.Equal("TN-001", model[0].TenderNumber);
        }

        [Fact]
        public async Task CreateTender_ReturnResult()
        {
            // Arrange
            var categoryList = new List<TenderCategoryDTO>
            {
                new TenderCategoryDTO { Id = 1, Name = "Construction" },
                new TenderCategoryDTO { Id = 2, Name = "IT Services" }
            };

            var partnerList = new List<PartnerDTO>
            {
                new PartnerDTO { Id = 1, Name = "Partner A" },
                new PartnerDTO { Id = 2, Name = "Partner B" }
            };

            var mockCategoryService = new Mock<ITenderCategoryService>();
            mockCategoryService
                .Setup(service => service.GetAllAsync<List<TenderCategoryDTO>>())
                .ReturnsAsync(categoryList);

            var mockPartnerService = new Mock<IPartnerService>();
            mockPartnerService
                .Setup(service => service.GetAllAsync<List<PartnerDTO>>())
                .ReturnsAsync(partnerList);

            // Act
            var controller = new TenderController(
            Mock.Of<ITenderService>(),
            Mock.Of<IMapper>(),
            mockCategoryService.Object,
            mockPartnerService.Object
            );

            var result = await controller.CreateTender();

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult.ViewData["CategoryList"]);
            Assert.IsType<SelectList>(viewResult.ViewData["CategoryList"]);
            var categorySelectList = viewResult.ViewData["CategoryList"] as SelectList;
            Assert.Equal(2, categorySelectList.Count());
            Assert.NotNull(viewResult.ViewData["PartnerList"]);
            Assert.IsType<SelectList>(viewResult.ViewData["PartnerList"]);
            var partnerSelectList = viewResult.ViewData["PartnerList"] as SelectList;
            Assert.Equal(2, partnerSelectList.Count());
        }
    }
}

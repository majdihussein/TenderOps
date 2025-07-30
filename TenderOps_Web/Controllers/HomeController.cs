using System.Diagnostics;
using AutoMapper;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TenderOps_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITenderService _tenderService;
        private readonly IMapper _mapper;
        public HomeController(ITenderService tenderService, IMapper mapper)
        {
            _tenderService = tenderService;
            _mapper = mapper;
        }


        public async Task<IActionResult> Index()
        {
            List<TenderDTO> list = new();

            var response = await _tenderService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess && response.Result != null)
            {
                list = JsonConvert.DeserializeObject<List<TenderDTO>>(response.Result.ToString());
                list = list
                    .Where(t => t.TenderEndDate == null || t.TenderEndDate >= DateTime.Now)
                    .ToList();
            }

            return View(list);
        }

    }
}

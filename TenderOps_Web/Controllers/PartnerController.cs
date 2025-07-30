using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Controllers
{
    public class PartnerController : Controller
    {
        private readonly IPartnerService _PartnerService;
        private readonly IMapper _mapper;
        public PartnerController(IMapper mapper, IPartnerService PartnerService)
        {
            _PartnerService = PartnerService;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexPartner()
        {
            List<PartnerDTO> list = new();

            var response = await _PartnerService.GetAllAsync<APIResponse>();

            if (response != null && response.IsSuccess && response.Result != null)
            {
                list = JsonConvert.DeserializeObject<List<PartnerDTO>>
                    (Convert.ToString(response.Result.ToString()));
            }
            else
            {
                TempData["Error"] = "Error in uploading Partner";
            }
                return View(list);
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> CreatePartner()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartner(PartnerCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _PartnerService.CreateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Partner created successfully!";
                    return RedirectToAction(nameof(IndexPartner));
                }
                TempData["error"] = "Error creating Partner!";
            }
            else
            {
                TempData["error"] = "Validation error!";
            }
            return View(model);
        }

        //[Authorize(Roles = "superadmin")]
        //public async Task<IActionResult> UpdatePartner(int partnerId)
        //{
        //    var response = await _PartnerService.GetAsync<APIResponse>(partnerId);
        //    if (response != null && response.IsSuccess && response.Result != null)
        //    {
        //        PartnerDTO model = JsonConvert.DeserializeObject<PartnerDTO>(Convert.ToString(response.Result));
        //        return View(_mapper.Map<PartnerUpdateDTO>(model));
        //    }
        //    return NotFound();
        //}

        //[HttpPost]
        //[Authorize(Roles = "superadmin")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdatePartner(PartnerUpdateDTO model)
        //{

        //        if (ModelState.IsValid)
        //        {
        //            var response = await _PartnerService.UpdateAsync<APIResponse>(model);
        //            TempData["success"] = "Tender Updated successfully!";


        //            if (response != null && response.IsSuccess)
        //            {
        //                return RedirectToAction(nameof(IndexPartner));
        //            }
        //        }
        //        TempData["error"] = "Error updating tender!";
        //        return View(model);
        //}

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> DeletePartner(int partnerId)
        {
            var response = await _PartnerService.GetAsync<APIResponse>(partnerId);
            if (response != null && response.IsSuccess)
            {
                PartnerDTO model = JsonConvert.DeserializeObject<PartnerDTO>
                    (Convert.ToString(response.Result));
                return View(model);
            }
            TempData["error"] = "Partner not found!";
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePartner(PartnerDTO model)
        {
                var response = await _PartnerService.DeleteAsync<APIResponse>(model.Id);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Partner Deleted successfully!";
                    return RedirectToAction(nameof(IndexPartner));
                }
                TempData["error"] = "Error deleting Partner!";
                return View(model);
        }
    }
}

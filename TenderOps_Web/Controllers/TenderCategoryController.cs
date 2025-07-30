using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Controllers
{
    public class TenderCategoryController : Controller
    {
        private readonly ITenderService _tenderService;
        public readonly ITenderCategoryService _tenderCategoryService;
        private readonly IMapper _mapper;
        public TenderCategoryController(ITenderService tenderervice, IMapper mapper, ITenderCategoryService tenderCategoryService)
        {
            _tenderService = tenderervice;
            _mapper = mapper;
            _tenderCategoryService = tenderCategoryService;
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> IndexTenderCategory()
        {
            List<TenderCategoryDTO> list = new();

            var response = await _tenderCategoryService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess && response.Result != null)
            {
                list = JsonConvert.DeserializeObject<List<TenderCategoryDTO>>
                    (Convert.ToString(response.Result.ToString()));
            }
            else
            {
                TempData["Error"] = "Error in uploading Category";
            }
             return View(list);
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> CreateTenderCategory()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTenderCategory(TenderCategoryCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _tenderCategoryService.CreateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Tender created successfully!";
                    return RedirectToAction(nameof(IndexTenderCategory));
                }
            }
            TempData["error"] = "Error creating Category!";
            return View(model);
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> UpdateTenderCategory(int categoryId)
        {
            var response = await _tenderCategoryService.GetAsync<APIResponse>(categoryId);
            if (response != null && response.IsSuccess)
            {
                TenderCategoryDTO model = JsonConvert.DeserializeObject<TenderCategoryDTO>(Convert.ToString(response.Result));
                return View(_mapper.Map<TenderCategoryUpdateDTO>(model));
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTenderCategory(TenderCategoryUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _tenderCategoryService.UpdateAsync<APIResponse>(model);
                TempData["success"] = "Tender Updated successfully!";


                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexTenderCategory));
                }
            }
            TempData["error"] = "Error updating tender!";
            return View(model);
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> DeleteTenderCategory(int categoryId)
        {
            var response = await _tenderService.GetAsync<APIResponse>(categoryId);
            if (response != null && response.IsSuccess)
            {
                TenderCategoryDTO model = JsonConvert.DeserializeObject<TenderCategoryDTO>
                    (Convert.ToString(response.Result));
                return View(model);
            }
            TempData["error"] = "Tender not found!";
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTenderCategory(TenderCategoryDTO model)
        {
                var response = await _tenderService.DeleteAsync<APIResponse>(model.Id);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Tender Deleted successfully!";
                    return RedirectToAction(nameof(IndexTenderCategory));
                }
                TempData["error"] = "Error deleting Tender!";
                return View(model);
        }
    }
}

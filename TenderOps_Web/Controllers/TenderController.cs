using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Services.IServices;

namespace TenderOps_Web.Controllers
{
    public class TenderController : Controller
    {
        private readonly ITenderService _tenderService;
        private readonly ITenderCategoryService _tenderCategoryService;
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        public TenderController(ITenderService tenderservice, IMapper mapper
            , ITenderCategoryService tenderCategoryService
            , IPartnerService partnerService)
        {
            _tenderService = tenderservice;
            _tenderCategoryService = tenderCategoryService;
            _mapper = mapper;
            _partnerService = partnerService;
        }

        public async Task<IActionResult> IndexTender()
        {
            List<TenderDTO> list = new();

            var response = await _tenderService.GetAllAsync<APIResponse>();

            if (response != null && response.IsSuccess && response.Result != null)
            {
                list = JsonConvert.DeserializeObject<List<TenderDTO>>
                    (Convert.ToString(response.Result.ToString()));

            }
            else
            {
                TempData["Error"] = "Error in uploading Tender";
            }
                return View(list);
        }

        [Authorize(Roles = "admin, superadmin")]
        public async Task<IActionResult> AdminTenderList(bool? withoutInvoice = null)
        {
            List<TenderDTO> list = new();

            var response = await _tenderService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess && response.Result != null)
            {
                var allTenders = JsonConvert.DeserializeObject<List<TenderDTO>>(Convert.ToString(response.Result));

                list = withoutInvoice == true
                    ? allTenders.Where(t => t.Invoice == null).ToList()
                    : allTenders;
            }
            else
            {
                TempData["error"] = "Error in uploading Tender!";
            }

            return View(list);
        }

        [Authorize(Roles = "admin, superadmin")]
        public async Task<IActionResult> CreateTender()
        {
            var categoryList = await _tenderCategoryService.GetAllAsync<List<TenderCategoryDTO>>();
            ViewBag.CategoryList = new SelectList(categoryList, "Id", "Name");

            var partnerList = await _partnerService.GetAllAsync<List<PartnerDTO>>();
            ViewBag.PartnerList = new SelectList(partnerList, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTender(TenderCreateDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _tenderService.CreateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Tender created successfully!";
                    return RedirectToAction(nameof(IndexTender));
                }
                TempData["error"] = "Error creating Tender!";
            }
            else
            {
                TempData["error"] = "Validation error!";
            }

            // جلب التصنيفات
            var categoryResponse = await _tenderCategoryService.GetAllAsync<APIResponse>();
            List<TenderCategoryDTO> categoryList = new List<TenderCategoryDTO>();
            if (categoryResponse != null && categoryResponse.IsSuccess)
            {
                categoryList = JsonConvert.DeserializeObject<List<TenderCategoryDTO>>(Convert.ToString(categoryResponse.Result));
            }
            ViewBag.CategoryList = new SelectList(categoryList, "Id", "Name", model.TenderCategoryId);

            // جلب الشركاء
            var partnerResponse = await _partnerService.GetAllAsync<APIResponse>();
            List<PartnerDTO> partnerList = new List<PartnerDTO>();
            if (partnerResponse != null && partnerResponse.IsSuccess)
            {
                partnerList = JsonConvert.DeserializeObject<List<PartnerDTO>>(Convert.ToString(partnerResponse.Result));
            }
            ViewBag.PartnerList = new SelectList(partnerList, "Id", "Name", model.PartnerId);

            return View(model);
        }

        [Authorize(Roles = "admin, superadmin")]
        public async Task<IActionResult> UpdateTender(int tenderId)
        {
            var response = await _tenderService.GetAsync<APIResponse>(tenderId);
            if (response != null && response.IsSuccess)
            {
                var jsonString = Convert.ToString(response.Result);
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    return BadRequest("Tender data not found.");
                }

                TenderDTO model = JsonConvert.DeserializeObject<TenderDTO>(jsonString);
                var updateModel = _mapper.Map<TenderUpdateDTO>(model);

                // جلب الشركاء
                var partnersResponse = await _partnerService.GetAllAsync<APIResponse>();
                if (partnersResponse != null && partnersResponse.IsSuccess)
                {
                    var partners = JsonConvert.DeserializeObject<List<PartnerDTO>>(Convert.ToString(partnersResponse.Result));
                    ViewBag.PartnerList = new SelectList(partners, "Id", "Name", updateModel.PartnerId);
                }
                else
                {
                    ViewBag.PartnerList = new SelectList(new List<PartnerDTO>(), "Id", "Name");
                }

                var categoriesResponse = await _tenderCategoryService.GetAllAsync<APIResponse>();
                if (categoriesResponse != null && categoriesResponse.IsSuccess)
                {
                    var categories = JsonConvert.DeserializeObject<List<TenderCategoryDTO>>(Convert.ToString(categoriesResponse.Result));
                    ViewBag.CategoryList = new SelectList(categories, "Id", "Name", updateModel.TenderCategoryId);
                }
                else
                {
                    ViewBag.CategoryList = new SelectList(new List<TenderCategoryDTO>(), "Id", "Name");
                }

                return View(updateModel);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin, superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTender(TenderUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                // أولاً: جلب التصنيفات للتحقق منها
                var categoriesResponse = await _tenderCategoryService.GetAllAsync<APIResponse>();
                List<TenderCategoryDTO> categories = new List<TenderCategoryDTO>();
                if (categoriesResponse != null && categoriesResponse.IsSuccess)
                {
                    categories = JsonConvert.DeserializeObject<List<TenderCategoryDTO>>(Convert.ToString(categoriesResponse.Result));
                }

                // تحقق هل الـ TenderCategoryId موجود ضمن التصنيفات
                bool categoryExists = categories.Any(c => c.Id == model.TenderCategoryId);
                if (!categoryExists)
                {
                    ModelState.AddModelError("TenderCategoryId", "Invalid tender category selected.");
                    // إعادة تعيين القائمة للعرض في الـ View
                    ViewBag.CategoryList = new SelectList(categories, "Id", "Name", model.TenderCategoryId);
                    return View(model);
                }

                // إذا التصنيف صحيح، نكمل التحديث
                var response = await _tenderService.UpdateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Tender Updated successfully!";
                    return RedirectToAction(nameof(IndexTender));
                }
                TempData["error"] = "Error updating tender!";
            }
            else
            {
                TempData["error"] = "Validation error!";
            }

            // إعادة تعبئة التصنيفات عند الفشل في التحقق أو التحديث
            var categoriesResponse2 = await _tenderCategoryService.GetAllAsync<APIResponse>();
            if (categoriesResponse2 != null && categoriesResponse2.IsSuccess)
            {
                var categories2 = JsonConvert.DeserializeObject<List<TenderCategoryDTO>>(Convert.ToString(categoriesResponse2.Result));
                ViewBag.CategoryList = new SelectList(categories2, "Id", "Name", model.TenderCategoryId);
            }
            else
            {
                ViewBag.CategoryList = new SelectList(new List<TenderCategoryDTO>(), "Id", "Name");
            }

            // تحميل الشركاء
            var partnersResponse = await _partnerService.GetAllAsync<APIResponse>();
            if (partnersResponse != null && partnersResponse.IsSuccess)
            {
                var partners = JsonConvert.DeserializeObject<List<PartnerDTO>>(Convert.ToString(partnersResponse.Result));
                ViewBag.PartnerList = new SelectList(partners, "Id", "Name", model.PartnerId);
            }
            else
            {
                ViewBag.PartnerList = new SelectList(new List<PartnerDTO>(), "Id", "Name");
            }
            return View(model);
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> DeleteTender(int tenderId)
        {
            var response = await _tenderService.GetAsync<APIResponse>(tenderId);
            if (response != null && response.IsSuccess)
            {
                TenderDTO model = JsonConvert.DeserializeObject<TenderDTO>
                    (Convert.ToString(response.Result));
                return View(model);
            }
            TempData["error"] = "Tender not found!";
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTender(TenderDTO model)
        {
                var response = await _tenderService.DeleteAsync<APIResponse>(model.Id);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Tender Deleted successfully!";
                    return RedirectToAction(nameof(IndexTender));
                }
                TempData["error"] = "Error deleting Tender!";
                return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTendersByPartner(int partnerId)
        {
            var tenderList = await _tenderService.GetTenderPartner<List<TenderDTO>>(partnerId);

            if (tenderList == null)
                tenderList = new List<TenderDTO>();

            return Json(tenderList);
        }


        //[HttpGet]
        //public async Task<IActionResult> TenderPartner(int partnerId)
        //{
        //    var tenderList = await _tenderService.GetTenderPartner<List<TenderDTO>>(partnerId);

        //    if (tenderList != null && tenderList.Any())
        //    {
        //        return View(tenderList);
        //    }
        //    return RedirectToAction("Index", "Partner");
        //}



    }
}

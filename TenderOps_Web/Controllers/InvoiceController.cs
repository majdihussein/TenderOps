using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using TenderOps_Utility;
using TenderOps_Web.Models;
using TenderOps_Web.Models.Dto;
using TenderOps_Web.Models.VM;
using TenderOps_Web.Services;
using TenderOps_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace TenderOps_Web.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ITenderService _tenderService;
        private readonly IMapper _mapper;
        public InvoiceController(IInvoiceService invoiceervice, IMapper mapper, ITenderService tenderService)
        {
            _invoiceService = invoiceervice;
            _mapper = mapper;
            _tenderService = tenderService;
        }

        [Authorize(Roles = "admin, superadmin")]
        public async Task<IActionResult> IndexInvoice()
        {
            List<InvoiceDTO> list = new();

            var response = await _invoiceService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<InvoiceDTO>>
                    (Convert.ToString(response.Result));
            }
            return View(list);
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> CreateInvoice()
        {
            InvoiceCreateVM invoiceVM = new();
            var response = await _tenderService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                var tenderList = JsonConvert.DeserializeObject<List<TenderDTO>>(Convert.ToString(response.Result));

                invoiceVM.TenderList = tenderList
                    .Where(i => i.Invoice == null) //  فقط العطاءات اللي ما إلها فاتورة
                    .Select(i => new SelectListItem
                    {
                        Text = i.TenderName,
                        Value = i.Id.ToString()
                    });
            }
            return View(invoiceVM);
        }

        [Authorize(Roles = "superadmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvoice(InvoiceCreateVM model)
        {
            if (ModelState.IsValid)
            {
                // جلب العطاء المرتبط بالفاتورة
                var tenderResp = await _tenderService.GetAsync<APIResponse>(model.InvoiceCreateDTO.TenderId);
                if (tenderResp != null && tenderResp.IsSuccess)
                {
                    var tender = JsonConvert.DeserializeObject<TenderDTO>(Convert.ToString(tenderResp.Result));

                    // إضافة CreatedBy و UserId في DTO (تأكد من وجودهما في DTO)
                    model.InvoiceCreateDTO.CreatedBy = User.Identity.Name;
                    model.InvoiceCreateDTO.UserId = tender.CreatedByUserId; // أو الخاصية المناسبة في TenderDTO التي تشير لمن أنشأ العطاء

                    var response = await _invoiceService.CreateAsync<APIResponse>(model.InvoiceCreateDTO);
                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Invoice created successfully!";
                        return RedirectToAction(nameof(IndexInvoice));
                    }
                    else
                    {
                        TempData["error"] = (response.ErrorMessage != null && response.ErrorMessage.Count > 0) ?
                            response.ErrorMessage[0] : "Error Encountered";
                    }
                }
                else
                {
                    TempData["error"] = "Failed to get Tender information!";
                }
            }

            var resp = await _tenderService.GetAllAsync<APIResponse>();
            if (resp != null && resp.IsSuccess)
            {
                var tenders = JsonConvert.DeserializeObject<List<TenderDTO>>(Convert.ToString(resp.Result));

                model.TenderList = tenders
                    .Where(i => i.Invoice == null)
                    .Select(i => new SelectListItem
                    {
                        Text = i.TenderName,
                        Value = i.Id.ToString()
                    });
            }
            return View(model);
        }

        [Authorize(Roles = "superadmin")]
        public async Task<IActionResult> DeleteInvoice(int invoiceNo)
        {
            TempData["error"] = "لا يمكن حذف الفاتورة بعد إنشائها.";
            return RedirectToAction(nameof(IndexInvoice));
        }

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice(InvoiceDTO model)
        {
            TempData["error"] = "لا يمكن حذف رقم الفيلا حفاظًا على سلامة البيانات.";
            return RedirectToAction(nameof(IndexInvoice));
        }
    }
}

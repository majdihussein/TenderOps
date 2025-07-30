using System.Net;
using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TenderOps_API.Data;
using TenderOps_API.Models;
using TenderOps_API.Models.Dto;
using TenderOps_API.Repository;
using TenderOps_API.Repository.IRepository;

namespace TenderOps_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/TenderAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class TenderAPIController : ControllerBase
    {
        private readonly ITenderRepository _dbTender;
        private readonly IPartnerRepository _dbPartner;
        private readonly ITenderCategoryRepository _dbTenderCategory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public TenderAPIController(ITenderRepository dbTender, IMapper mapper
            , IPartnerRepository dbPartner, ITenderCategoryRepository dbTenderCategory, UserManager<ApplicationUser> userManager)
        {
            _dbTender = dbTender;
            _mapper = mapper;
            _response = new();
            _dbPartner = dbPartner;
            _dbTenderCategory = dbTenderCategory;
            _userManager = userManager;
        }

        [HttpGet]
        //[ResponseCache(CacheProfileName = "Default30")] // لتخزين المعلومات لمدة 30 ثانية وهذا اسم البروفايل
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<APIResponse>> GetTenders([FromQuery] string? tendernumber,
            [FromQuery] string? search, int pageSize = 2, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Tender> tinderlist;

                if (tendernumber != null)
                {
                    tinderlist = await _dbTender.GetAllAsync(u => u.TenderNumber == tendernumber,
                    includeProperties: "TenderCategory,Partner,CreatedByUser",
                    pageSize: pageSize, pageNumber: pageNumber);

                }
                else
                {
                    tinderlist = await _dbTender.GetAllAsync(
                    includeProperties: "TenderCategory,Partner,CreatedByUser",
                    pageSize: pageSize, pageNumber: pageNumber);
                    ;
                }

                if (!string.IsNullOrEmpty(search))
                {
                    tinderlist = tinderlist.Where(u => u.TenderName.ToLower().Contains(search));
                }

                Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

                _response.Result = _mapper.Map<List<TenderDTO>>(tinderlist);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };

            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetTender")]
        //[ResponseCache(Location =ResponseCacheLocation.None,NoStore = true)] // لمنع التخزين للمعلومات الحساسة
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetTender(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var tender = await _dbTender.GetAsync(u => u.Id == id, includeProperties: "CreatedByUser,Partner,TenderCategory");
                if (tender == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<TenderDTO>(tender);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("GetTendersWithoutInvoices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "superadmin")]
        public async Task<ActionResult<APIResponse>> GetTendersWithoutInvoices()
        {
            try
            {
                var tenders = await _dbTender.GetAllAsync(t => t.Invoice == null, includeProperties: "Partner,CreatedByUser,TenderCategory");
                var dtoList = _mapper.Map<List<TenderDTO>>(tenders);

                _response.Result = dtoList;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_response);
            }
        }

        [HttpPost]
        [Authorize(Roles = "superadmin, admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateTender([FromForm] TenderCreateDTO createDTO)
        {
            try
            {
                if (await _dbTender.GetAsync(u => u.TenderName.ToLower() == createDTO.TenderName.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Tender already exist");
                    return BadRequest(ModelState);
                }

                var partner = await _dbPartner.GetAsync(p => p.Id == createDTO.PartnerId);
                if (partner == null)
                {
                    ModelState.AddModelError("ErrorMessage", $"Partner with ID {createDTO.PartnerId} not found");
                    return BadRequest(ModelState);
                }

                var tenderCategory = await _dbTenderCategory.GetAsync(tc => tc.Id == createDTO.TenderCategoryId);
                if (tenderCategory == null)
                {
                    ModelState.AddModelError("ErrorMessage", $"TenderCategory with ID {createDTO.TenderCategoryId} not found");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest();
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.Users.Include(u => u.Partner)
                                   .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || user.Partner == null)
                {
                    ModelState.AddModelError("ErrorMessage", "User has no associated partner");
                    return BadRequest(ModelState);
                }

                Tender tender = _mapper.Map<Tender>(createDTO);
                tender.CreatedByUserId = userId;
                tender.CreatedDate = DateTime.UtcNow;
                tender.PartnerId = user.Partner.Id;

                await _dbTender.CreateAsync(tender);

                // If the image is provided, save it to the server
                if (createDTO.Image != null)
                {
                    string fileName = tender.Id + Path.GetExtension(createDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo file = new FileInfo(directoryLocation);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        createDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    tender.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    tender.ImageLocalPath = filePath;
                }
                else
                {
                    tender.ImageUrl = "https://placehold.co/600x401";
                }

                await _dbTender.UpdateAsync(tender); // عملنا انشاء ومن ثم تعديل عشان الانشاء الجديد يوخذ اي دي قبل الصورة
                _response.Result = _mapper.Map<TenderDTO>(tender);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetTender", new { id = tender.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteTender")]
        [Authorize(Roles = "superadmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteTender(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var tender = await _dbTender.GetAsync(u => u.Id == id);
                if (tender == null)
                {
                    return NotFound();
                }

                // If the image exists, delete it from the server
                if (!string.IsNullOrEmpty(tender.ImageLocalPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), tender.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePath);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                await _dbTender.RemoveAsync(tender);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id:int}", Name = "UpdateTender")]
        [Authorize(Roles = "admin, superadmin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateTender(int id, [FromForm] TenderUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

                var existingTender = await _dbTender.GetAsync(t => t.Id == id);
                if (existingTender == null)
                {
                    return NotFound($"Tender with ID {id} not found.");
                }

                // تحديث الحقول من DTO
                existingTender.TenderName = updateDTO.TenderName;
                existingTender.TenderNumber = updateDTO.TenderNumber;
                existingTender.Description = updateDTO.Description;
                existingTender.Location = updateDTO.Location;
                existingTender.Status = updateDTO.Status;
                existingTender.TenderStartDate = updateDTO.TenderStartDate;
                existingTender.TenderEndDate = updateDTO.TenderEndDate;
                existingTender.TenderCategoryId = updateDTO.TenderCategoryId;
                existingTender.PartnerId = updateDTO.PartnerId;

                // If the image is provided, save it to the server
                if (updateDTO.Image != null)
                {
                    if (!string.IsNullOrEmpty(existingTender.ImageLocalPath))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), existingTender.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePath);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    string fileName = id + Path.GetExtension(updateDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        updateDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    existingTender.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    existingTender.ImageLocalPath = filePath;
                }
                else
                {
                    existingTender.ImageUrl = "https://placehold.co/600x401";
                }

                await _dbTender.UpdateAsync(existingTender);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialTender")]
        [Authorize(Roles = "superadmin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialTender(int id, JsonPatchDocument<TenderUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            var tender = await _dbTender.GetAsync(u => u.Id == id, tracked: false);

            TenderUpdateDTO tenderDTO = _mapper.Map<TenderUpdateDTO>(tender);

            if (tender == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(tenderDTO, ModelState);

            Tender model = _mapper.Map<Tender>(tenderDTO);

            await _dbTender.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();


        }

        [HttpGet("GetTenderPartner/{partnerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TenderDTO>>> GetTenderPartner(int partnerId)
        {
            var tenders = await _dbTender.GetTenderPartnerAsync(partnerId);

            if (tenders == null || !tenders.Any())
                return NotFound();

            var tenderDTOs = _mapper.Map<IEnumerable<TenderDTO>>(tenders);

            return Ok(tenderDTOs);
        }


    }
}

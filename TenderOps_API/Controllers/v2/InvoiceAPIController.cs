using System.Net;
using System.Security.Claims;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenderOps_API.Data;
using TenderOps_API.Models;
using TenderOps_API.Models.Dto;
using TenderOps_API.Repository.IRepository;

namespace TenderOps_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/InvoiceAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class InvoiceAPIController : ControllerBase
    {
        private readonly IInvoiceRepository _dbInvoice;
        private readonly ITenderRepository _dbTender;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public InvoiceAPIController(IInvoiceRepository dbInvoice, IMapper mapper, ITenderRepository dbTender)
        {
            _dbInvoice = dbInvoice;
            _mapper = mapper;
            _response = new();
            _dbTender = dbTender;
        }

        [HttpGet("GetString")]
        [Authorize(Roles = "admin")]
        public IEnumerable<string> get()
        {
            return new string[] { "value1v2", "value2v2" };
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetInvoice()
        {
            try
            {
                IEnumerable<Invoice> invoicelist = await _dbInvoice.GetAllAsync(includeProperties: "Tender,User");
                _response.Result = _mapper.Map<List<InvoiceDTO>>(invoicelist);
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

        [HttpGet("{id:int}", Name = "GetInvoice")]
        [Authorize(Roles = "admin, superadmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetInvoice(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var invoice = await _dbInvoice.GetAsync(u => u.InvoiceId == id, includeProperties: "Tender,User");
                if (invoice == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<InvoiceDTO>(invoice);
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

        [HttpPost]
        [Authorize(Roles = "superadmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateInvoice([FromBody] InvoiceCreateDTO createDTO)
        {
            try
            {

                if (await _dbInvoice.GetAsync(u => u.InvoiceNumber == createDTO.InvoiceNumber) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Invoice Number already exist");
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

                var tenderExists = await _dbTender.GetAsync(t => t.Id == createDTO.TenderId);
                if (tenderExists == null)
                {
                    ModelState.AddModelError("ErrorMessage", "Tender Not Found");
                    return BadRequest(ModelState);
                }

                Invoice invoice = _mapper.Map<Invoice>(createDTO);

                invoice.InvoiceDate = DateTime.Now;

                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

                invoice.CreatedBy = userName;
                invoice.UserId = userId;

                await _dbInvoice.CreateAsync(invoice);
                _response.Result = _mapper.Map<InvoiceDTO>(invoice);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetInvoice", new { id = invoice.InvoiceId }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteInvoice")]
        [Authorize(Roles = "superadmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteInvoice(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var invoice = await _dbInvoice.GetAsync(u => u.InvoiceId == id);
                if (invoice == null)
                {
                    return NotFound();
                }
                await _dbInvoice.RemoveAsync(invoice);
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
    }
}

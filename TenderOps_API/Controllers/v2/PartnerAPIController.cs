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
    [Route("api/v{version:apiVersion}/PartnerAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class PartnerAPIController : ControllerBase
    {
        private readonly IPartnerRepository _dbPartner;
        private readonly ITenderRepository _dbTender;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public PartnerAPIController(IPartnerRepository dbPartner, IMapper mapper, ITenderRepository dbTender)
        {
            _dbPartner = dbPartner;
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
        public async Task<ActionResult<APIResponse>> GetPartners()
        {
            try
            {
                IEnumerable<Partner> partnerlist = await _dbPartner.GetAllAsync();
                _response.Result = _mapper.Map<List<PartnerDTO>>(partnerlist);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);

            }
        }

        [HttpGet("{id:int}", Name = "GetPartner")]
        [Authorize(Roles = "admin, superadmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPartner(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var partner = await _dbPartner.GetAsync(u => u.Id == id);
                if (partner == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<PartnerDTO>(partner);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreatePartner([FromBody] PartnerCreateDTO createDTO)
        {
            try
            {

                if (await _dbPartner.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Partner already exist");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest();
                }


                Partner partner = _mapper.Map<Partner>(createDTO);

                await _dbPartner.CreateAsync(partner);
                _response.Result = _mapper.Map<PartnerDTO>(partner);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetPartner", new { id = partner.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
                _response.StatusCode = HttpStatusCode.InternalServerError;
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

        [HttpDelete("{id:int}", Name = "DeletePartner")]
        [Authorize(Roles = "superadmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeletePartner(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var partner = await _dbPartner.GetAsync(u => u.Id == id);
                if (partner == null)
                {
                    return NotFound();
                }
                await _dbPartner.RemoveAsync(partner);
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

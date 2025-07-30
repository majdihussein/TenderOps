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
    [Route("api/v{version:apiVersion}/TenderCategoryAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class TenderCategoryAPIController : ControllerBase
    {
        private readonly ITenderCategoryRepository _dbTenderCategory;
        private readonly ITenderRepository _dbTender;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public TenderCategoryAPIController(ITenderCategoryRepository dbTenderCategory, IMapper mapper, ITenderRepository dbTender)
        {
            _dbTenderCategory = dbTenderCategory;
            _mapper = mapper;
            _response = new();
            _dbTender = dbTender;
        }

        [HttpGet("GetString")]
        public IEnumerable<string> get()
        {
            return new string[] { "value1v2", "value2v2" };
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetTenderCategories()
        {
            try
            {
                IEnumerable<TenderCategory> tenderCategorylist = await _dbTenderCategory.GetAllAsync();
                _response.Result = _mapper.Map<List<TenderCategoryDTO>>(tenderCategorylist);
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

        [HttpGet("{id:int}", Name = "GetTenderCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetTenderCategory(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var tenderCategory = await _dbTenderCategory.GetAsync(u => u.Id == id);
                if (tenderCategory == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<TenderCategoryDTO>(tenderCategory);
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
        public async Task<ActionResult<APIResponse>> CreateTenderCategory([FromBody] TenderCategoryCreateDTO createDTO)
        {
            try
            {
                if (await _dbTenderCategory.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Category already exist");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest();
                }

                TenderCategory tenderCategory = _mapper.Map<TenderCategory>(createDTO);

                await _dbTenderCategory.CreateAsync(tenderCategory);
                _response.Result = _mapper.Map<TenderCategoryDTO>(tenderCategory);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetTenderCategory", new { id = tenderCategory.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteTenderCategory")]
        [Authorize(Roles = "superadmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteCategory(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var tenderCategory = await _dbTenderCategory.GetAsync(u => u.Id == id);
                if (tenderCategory == null)
                {
                    return NotFound();
                }
                await _dbTenderCategory.RemoveAsync(tenderCategory);
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

        [HttpPut("{id:int}", Name = "UpdateTenderCategory")]
        [Authorize(Roles = "superadmin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateTenderCategory(int id, [FromBody]TenderCategoryUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }

                var existingCategory = await _dbTenderCategory.GetAsync(c => c.Id == id);
                if (existingCategory == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                existingCategory.Name = updateDTO.Name;


                await _dbTenderCategory.UpdateAsync(existingCategory);
                _response.StatusCode = HttpStatusCode.OK;
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

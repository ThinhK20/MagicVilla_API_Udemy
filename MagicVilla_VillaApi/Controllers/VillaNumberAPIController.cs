using AutoMapper;
using MagicVilla_VillaApi.Logging;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VillaNumberAPIController : Controller
	{
		private readonly IMapper _mapper;
		private readonly ILogging _logger;
		private readonly IVillaNumberRepository _db;
		private readonly IVillaRepository _dbVilla;
		protected APIResponse _response;

		public VillaNumberAPIController(IMapper mapper, ILogging logger, IVillaNumberRepository db, IVillaRepository dbVilla)
		{
			_mapper = mapper;
			_logger = logger;
			_db = db;
			_response = new();
			_dbVilla = dbVilla;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillaNumbers()
		{
			try
			{
				var villaNumbers = await _db.GetAllAsync(includeProperties: "Villa");
				_response.Result = _mapper.Map<IEnumerable<VillaNumberDTO>>(villaNumbers);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception err)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { err.ToString() };
			}
			return _response;
		}

		[HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]

		public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
		{
			try
			{
				if (villaNo <= 0)
				{
					_logger.Log("Get villa number error with id: " + villaNo, "error");
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}
				var villaNumber = await _db.GetAsync(x => x.VillaNo == villaNo);
				if (villaNumber == null)
				{
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}
				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception err)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { err.ToString() };
			}
			return _response;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
		{
			try
			{

				if (await _db.GetAsync(x => x.VillaNo == createDTO.VillaNo) != null)
				{
					_logger.Log("Villa number is exists with number: " + createDTO.VillaNo, "error");
					ModelState.AddModelError("ErrorMessages", "Villa number is already exists!");
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessages = new List<string>() { "Villa number is already exists with number: " + createDTO.VillaNo };
					return BadRequest(_response);
				}

				if (await _dbVilla.GetAsync(x => x.Id == createDTO.VillaID) == null)
				{
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessages = new List<string>() { "Villa ID is invalid !" };
					ModelState.AddModelError("ErrorMessages", "Villa ID is invalid !");
					return BadRequest(_response);
				}

				VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
				await _db.CreateAsync(villaNumber);
				_response.IsSuccess = true;
				_response.StatusCode = HttpStatusCode.OK;
				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				return CreatedAtAction("GetVillaNumber", new { villaNo = createDTO.VillaNo }, _response);
			}
			catch (Exception err)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { err.ToString() };
			}
			return _response;
		}

		[HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> DeleteVillaNumber([FromRoute] int villaNo)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}
				if (villaNo <= 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					ModelState.AddModelError("ErrorMessages", "Villa No is not valid !");
					return BadRequest(_response);
				}
				var villaNumber = await _db.GetAsync(x => x.VillaNo == villaNo);
				if (villaNumber == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					ModelState.AddModelError("ErrorMessages", "Villa number is not exists !");
					return NotFound(_response);
				}
				await _db.Remove(villaNumber);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception err)
			{
				_response.ErrorMessages = new List<string>() { err.ToString() };
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> UpdateVillaNumber([FromRoute] int villaNo, [FromBody] VillaNumberUpdateDTO updateDTO)
		{
			try
			{
				if (!ModelState.IsValid) { return BadRequest(ModelState); }
				if (updateDTO == null || updateDTO.VillaNo != villaNo)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return BadRequest(_response);
				}
				var villaNumber = await _db.GetAsync(x => x.VillaNo == villaNo, false);
				if (villaNumber == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					return NotFound(_response);
				}
				if (await _dbVilla.GetAsync(x => x.Id == updateDTO.VillaID) == null)
				{
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessages = new List<string>() { "Villa ID is invalid !" };
					ModelState.AddModelError("ErrorMessages", "Villa ID is invalid !");
					return BadRequest(_response);
				}
				VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

				await _db.UpdateAsync(model);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString() };
			}
			return _response;
		}

		[HttpPatch("{villaNo:int}", Name = "UpdatePartialVillaNumber")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int villaNo, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
		{
			try
			{
				if (patchDTO == null || villaNo <= 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return BadRequest(_response);
				}
				var villaNumber = await _db.GetAsync(x => x.VillaNo == villaNo, false);
				if (villaNumber == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					return NotFound(_response);
				}
				VillaNumberUpdateDTO updateDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
				patchDTO.ApplyTo(updateDTO, ModelState);
				VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);
				await _db.UpdateAsync(model);
				if (!ModelState.IsValid)
				{
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.NoContent;
					return BadRequest(_response);
				}
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(model);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string> { ex.ToString() };
			}
			return _response;
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Services;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;
using MovieTheater.Application.DTOs;
using MovieTheater.Web.Infrastructure;
using System.Security.Claims;

namespace MovieTheater.Web.ApiControllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly IHallService _hallService;

        public AdminApiController(IHallService hallService)
        {
            _hallService = hallService;
        }

        [HttpPost("hall/{hallId}/sector/{sectorId}/price")]
        public async Task<IActionResult> UpdateSectorPrice(long hallId, long sectorId, [FromBody] SectorPriceUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return ApiProblem.Bad("Invalid data", "Price update request model is not valid");

            if (request.SectorId != sectorId)
                return ApiProblem.Bad("Mismatched sector ID", "URL sectorId does not match body sectorId");

            try
            {
                var success = await _hallService.UpdateSectorPriceAsync(request.SectorId, request.Price);

                return success
                    ? NoContent()
                    : ApiProblem.NotFound("Sector not found", $"SectorId = {request.SectorId}");
            }
            catch (Exception ex)
            {
                return ApiProblem.Internal(ex.Message);
            }
        }

        [HttpDelete("hall/{hallId}/sector/{sectorId}")]
        public async Task<IActionResult> DeleteSector(long hallId, long sectorId)
        {
            try
            {
                var result = await _hallService.DeleteSectorAsync(hallId, sectorId);

                return result switch
                {
                    DeleteSectorResult.Success => NoContent(),
                    DeleteSectorResult.NotFound => ApiProblem.NotFound("Sector not found", $"Sector id = {sectorId}"),
                    DeleteSectorResult.OnlyOneSectorLeft => ApiProblem.Bad("Cannot delete last sector", "At least one sector must remain in the hall"),
                    DeleteSectorResult.ReassignFailed => ApiProblem.Internal("Failed to move seats to another sector"),
                    _ => ApiProblem.Internal("Unexpected error occurred")
                };
            }
            catch (Exception ex)
            {
                return ApiProblem.Internal(ex.Message);
            }
        }
    }

    public class SectorPriceUpdateRequest
    {
        public long SectorId { get; set; }
        public decimal Price { get; set; }
    }
}
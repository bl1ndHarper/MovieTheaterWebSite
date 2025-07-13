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
        private readonly ISessionService _sessionService;

        public AdminApiController(IHallService hallService, ISessionService sessionService)
        {
            _hallService = hallService;
            _sessionService = sessionService;
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

        [HttpGet("halls")]
        public async Task<IActionResult> GetAllHalls()
        {
            var halls = await _hallService.GetAllHallsWithSectorsAsync();
            return Ok(halls.Select(h => new { h.Id, h.Name }));
        }


        [HttpPost("sessions/create")]
        public async Task<IActionResult> CreateSessions([FromBody] List<SessionCreateDto> dtos)
        {
                if (dtos == null || !dtos.Any())
                    return ApiProblem.Bad("No sessions data ", "Немає даних сеансів");

                var success = await _sessionService.CreateSessionsAsync(dtos);

                if (success)
                    return NoContent();
                else
                    return BadRequest("не вдалося створити сеанси");
        }

        public class SectorPriceUpdateRequest
        {
            public long SectorId { get; set; }
            public decimal Price { get; set; }
        }
    }
}
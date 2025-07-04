using Microsoft.AspNetCore.Mvc;
using MovieTheater.Application.Services;
using MovieTheater.Application.Interfaces;
using MovieTheater.Web.ViewModels;
using MovieTheater.Application.DTOs;
using MovieTheater.Web.Infrastructure;

namespace MovieTheater.Web.ApiControllers
{
    [Route("")]
    public class SessionApiController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ISessionService _sessionService;

        public SessionApiController(
            IMovieService movieService,
            ISessionService sessionService)
        {
            _movieService = movieService;
            _sessionService = sessionService;
        }

        [HttpGet("api/movies/{movieId:long}/dates")]
        public async Task<IActionResult> AvailableDates(long movieId)
        {
            var dates = await _sessionService.GetAvailableSessionDatesAsync(movieId);
            return Ok(dates);
        }
    
        [HttpGet("api/movies/{movieId:long}/sessions/{day}")]
        public async Task<IActionResult> SessionsByDay(long movieId, string day)
        {
            var sessions = await _sessionService.GetMovieSessionsByDayAsync(movieId, day);
            return Ok(sessions);
        }
    
        [HttpGet("api/sessions/{id:long}")]
        public async Task<IActionResult> GetSession(long id)
        {
            var session = await _sessionService.GetSessionByIdAsync(id);
            return session == null
                ? ApiProblem.NotFound("Session not found", $"Session id = {id}")
                : Ok(session);
        }
        
        [HttpPost("api/sessions")]
        public async Task<IActionResult> CreateSession(MovieSessionDto dto)
        {
            if (!ModelState.IsValid)
                return ApiProblem.Bad("Bad request", "Invalid session data");

            var created = await _sessionService.CreateSessionAsync(dto);
            return Created("", created);
        }
    
        [HttpDelete("api/sessions/{id:long}")]
        public async Task<IActionResult> DeleteSession(long id)
        {
            return await _sessionService.DeleteSessionAsync(id)
                ? NoContent()
                : ApiProblem.NotFound("Session not found", $"Session id = {id}");
        }

        [HttpPut("api/sessions/{id:long}")]
        public async Task<IActionResult> UpdateSession(long id, MovieSessionDto dto)
        {
            if (!ModelState.IsValid)
                return ApiProblem.Bad("Bad request", "Invalid session data");

            return await _sessionService.UpdateSessionAsync(id, dto)
                ? NoContent()
                : ApiProblem.NotFound("Session not found", $"id = {id}");
        }
    }
}
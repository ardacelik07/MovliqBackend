using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.Entity;
using RunningApplicationNew.Helpers;
using System;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using RunningApplicationNew.RepositoryLayer;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using RunningApplicationNew.Hubs;

namespace RunningApplicationNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IRaceRoomRepository _raceRoomRepository;
        private readonly IUserResultsRepository _userResultsRepository;
        private readonly IHubContext<RaceHub> _hubContext;
        private readonly IJwtHelper _jwtHelper;
        private readonly IEmailHelper _emailhelper;
        private readonly ILeaderBoardRepository _leaderBoardRepository;


        public LeaderBoardController(IUserRepository userRepository, IRaceRoomRepository raceRoomRepository, IJwtHelper jwtHelper, IEmailHelper emailHelper, IUserResultsRepository userResultsRepository, IHubContext<RaceHub> hubContext,ILeaderBoardRepository leaderBoardRepository)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailhelper = emailHelper;
            _raceRoomRepository = raceRoomRepository;
            _userResultsRepository = userResultsRepository;
            _hubContext = hubContext;
            _leaderBoardRepository = leaderBoardRepository;
        }


        [HttpGet("GetAllLeaderboard")]
        public async Task<IActionResult> GetAllRooms(string type)
        {


            if(type== "indoor")
            {

                var leaderboardresult = await _leaderBoardRepository.GetAllLeaderBoardIndoor();

                return Ok(leaderboardresult);
            }
            else if(type == "outdoor"){
                var leaderboardresult = await _leaderBoardRepository.GetAllLeaderBoardOutdoor();

                return Ok(leaderboardresult);
            }

            return BadRequest("table can not load");
           


        }

    
      [HttpGet("GetUserByIdLeaderBoardRanks")]
        public async Task<IActionResult> GetUserByIdLeaderBoardRanks()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                    return Unauthorized("Token bulunamadý.");

                // Token'ý doðrula ve email bilgisi al
                var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
                if (emailFromToken == null)
                    return Unauthorized("Geçersiz token.");

                var user = await _userRepository.GetByEmailAsync(emailFromToken);
                if (user == null)
                    return NotFound("Kullanýcý bulunamadý.");

                // await ekledik
                var ranks = await _leaderBoardRepository.GetLeaderboardRankById(user.Id);

                return Ok(ranks);
            }
            catch (Exception ex)
            {
                // Hata yönetimi ekledik
                return StatusCode(500, $"Bir hata oluþtu: {ex.Message}");
            }
        }

    }


}
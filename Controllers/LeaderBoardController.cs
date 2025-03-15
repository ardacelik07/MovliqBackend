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

        public LeaderBoardController(IUserRepository userRepository, IRaceRoomRepository raceRoomRepository, IJwtHelper jwtHelper, IEmailHelper emailHelper, IUserResultsRepository userResultsRepository, IHubContext<RaceHub> hubContext)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailhelper = emailHelper;
            _raceRoomRepository = raceRoomRepository;
            _userResultsRepository = userResultsRepository;
            _hubContext = hubContext;
        }
    }
    }
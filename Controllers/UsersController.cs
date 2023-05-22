using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tienda.Entidades;
using Tienda.Recursos;

namespace Tienda.Controllers
{
    [ApiController]
    [Route("api/users"), Authorize]
    public class UsersController : ControllerBase
    {
        private ApplicationDbContext db;

        public IConfiguration Configuration { get; }

        private readonly IMapper _mapper;

        public UsersController(ApplicationDbContext dbContext, IConfiguration configuration, IMapper mapper)
        {
            db = dbContext;
            Configuration = configuration;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult Get() => Ok(_mapper.Map<List<User>, List<UserResource>>(db.Users.ToList()));
    }
}

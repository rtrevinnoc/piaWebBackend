using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Monitores.Entidades;

namespace Monitores.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private ApplicationDbContext db;

        public IConfiguration Configuration { get; }

        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            db = dbContext;
            Configuration = configuration;
        }

        [HttpGet]
        public ActionResult<List<Branch>> Get() => db.Branches.Include(r => r.Company).ToList();


        [HttpPost]
        [Route("login")]
        public ActionResult Login([FromBody] Login user)
        {
            if (user is null)
            {
                return BadRequest();
            }
            if (user.UserName == "rtc" && user.Password == "123")
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: Configuration["JWT:ValidIssuer"],
                    audience: Configuration["JWT:ValidAudience"],
                    claims: new List<Claim>() {
                            new Claim("id", Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Email, "rtrevinnoc@hotmail.com")
                        },
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new JWTResponse { Token = tokenString });
            }
            return Unauthorized();
        }
    }
}

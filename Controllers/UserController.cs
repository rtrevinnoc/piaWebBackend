using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Monitores.Entidades;
using Monitores.Recursos;

namespace Monitores.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private ApplicationDbContext db;

        public IConfiguration Configuration { get; }

        private readonly IMapper _mapper;

        private readonly IDataProtectionProvider _provider;
        private readonly IDataProtector protector;

        public UserController(
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            IMapper mapper,
            IDataProtectionProvider provider
        ) {
            db = dbContext;
            Configuration = configuration;
            _mapper = mapper;
            _provider = provider;
            protector = _provider.CreateProtector(Configuration["ProtectionPurpose"]);
        }
        
        [NonAction]
        public JWTResponse CreateToken(UserSign user) {
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
            return new JWTResponse { Token = tokenString };
        }

        [HttpPost]
        public ActionResult SignUp([FromBody] UserSign credentials)
        {
            if (credentials is null)
            {
                return BadRequest();
            }

            var existingUser = db.Users.FirstOrDefault(x => x.UserName == credentials.UserName);

            if (existingUser != null) {

                if (protector.Unprotect(existingUser.Password) == credentials.Password) {
                    return Ok(CreateToken(credentials));
                } else {
                    return Unauthorized();
                }
            } else {
                var newUser = _mapper.Map<UserSign, User>(credentials);
                newUser.Password = protector.Protect(credentials.Password);
                db.Users.Add(newUser);
                db.SaveChanges();

                return Ok(CreateToken(credentials));
            }
        }


        [HttpGet]
        public ActionResult SignIn([FromQuery] UserSign credentials)
        {
            if (credentials is null)
            {
                return BadRequest();
            }

            var existingUser = db.Users.FirstOrDefault(x => x.UserName == credentials.UserName);

            if (protector.Unprotect(existingUser.Password) == credentials.Password) {
                return Ok(CreateToken(credentials));
            }

            return Unauthorized();
        }
    }
}

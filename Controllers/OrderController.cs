using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tienda.Entidades;
using Tienda.Recursos;

namespace Tienda.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private ApplicationDbContext db;

        public IConfiguration Configuration { get; }

        private readonly IMapper _mapper;

        private readonly IDataProtectionProvider _provider;
        private readonly IDataProtector protector;

        private SmtpClient smtpClient;

        public OrderController(
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
            smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(Configuration["Email:Address"], Configuration["Email:Password"]),
                EnableSsl = true,
            };
        }

        [HttpPut()]
        [Authorize]
        public ActionResult UpdateOrder([FromQuery] OrderResource orderResource)
        {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;
            var userEMail = User.FindFirst(c=>c.Type==ClaimTypes.Email)?.Value;

            if (userRole == "admin") {
                var order = db.Orders.Find(orderResource.OrderId);
                order.Status = orderResource.Status;

                db.SaveChanges();

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Configuration["Email:Address"]),
                    Subject = string.Format("Actualizacion de Orden {0}", order.Id),
                    Body = string.Format("El estado de su pedido ha cambiado a {0}", order.Status),
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(new MailAddress(userEMail));

                smtpClient.Send(mailMessage);

                // return Ok(_mapper.Map<ICollection<Product>, ICollection<ProductResourceOut>>(user.Cart));
                return Ok(order);
            }

            return Unauthorized();
        }
    }
}

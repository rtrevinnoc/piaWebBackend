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
    [Route("api/orders"), Authorize]
    public class OrdersController : ControllerBase
    {
        private ApplicationDbContext db;

        public IConfiguration Configuration { get; }

        private readonly IMapper _mapper;

        public OrdersController(ApplicationDbContext dbContext, IConfiguration configuration, IMapper mapper)
        {
            db = dbContext;
            Configuration = configuration;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult Get() {
            var orders = db.Orders.ToList();
            foreach (Order order in orders) {
                order.Products = db.BoughtProducts.Where(x => x.OrderId == order.Id).ToList();
            }
            return Ok(orders);
        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult GetByUser([FromQuery] Guid userId) {
            var orders = db.Orders.ToList();
            foreach (Order order in orders) {
                order.Products = db.BoughtProducts.Where(x => x.User.Id == userId && x.OrderId == order.Id).ToList();
            }
            return Ok(orders);
        }
    }
}

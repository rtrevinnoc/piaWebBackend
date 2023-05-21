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
    [Route("api/products"), Authorize]
    public class ProductsController : ControllerBase
    {
        private ApplicationDbContext db;

        public IConfiguration Configuration { get; }

        private readonly IMapper _mapper;

        public ProductsController(ApplicationDbContext dbContext, IConfiguration configuration, IMapper mapper)
        {
            db = dbContext;
            Configuration = configuration;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Authorize]
        public ActionResult<List<ProductResourceIn>> Get() => Ok(_mapper.Map<List<Product>, List<ProductResourceOut>>(db.Products.ToList()));
    }
}

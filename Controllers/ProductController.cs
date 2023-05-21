using System.Security.Claims;
using System.Drawing;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Tienda.Entidades;
using Tienda.Recursos;

namespace Tienda.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private ApplicationDbContext db;

        public IConfiguration Configuration { get; }

        private readonly IMapper _mapper;

        private readonly IDataProtectionProvider _provider;
        private readonly IDataProtector protector;

        public ProductController(
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
        
        [HttpPost]
        [Authorize]
        public ActionResult RegisterProduct([FromForm] ProductResourceIn newProduct)
        {
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            if (userRole == "admin") {
                var product = _mapper.Map<ProductResourceIn, Product>(newProduct);
                db.Products.Add(product);
                db.SaveChanges();
                return Ok(product);
            }

            return Unauthorized();
        }

        [HttpGet("image")]
        public ActionResult RetrieveImage([FromQuery] string name)
        {
            Product product = db.Products.FirstOrDefault(x => x.Name == name);

            Image image;
            using (MemoryStream ms = new MemoryStream(product.Image))
            {
                image = Image.FromStream(ms);
            }

            return Ok(image);
        }
    }
}

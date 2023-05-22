using System.Security.Claims;
using System.Drawing;
using System.Data.Entity;
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

        [HttpGet]
        [Authorize]
        public ActionResult GetProduct([FromQuery] Guid productId)
        {
            Product product = db.Products.Find(productId);
            return Ok(product);
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

        [HttpPut]
        [Authorize]
        public ActionResult UpdateProduct([FromForm] ProductResourceUpdate updatedProduct)
        {
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            if (userRole == "admin") {
                Product existingProduct = db.Products.Find(updatedProduct.Id);
                var product = _mapper.Map<ProductResourceUpdate, ProductUpdate>(updatedProduct);

                if (product.Name != null) {
                    existingProduct.Name = product.Name;
                }

                if (product.Image != null) {
                    existingProduct.Image = product.Image;
                }

                if (product.Price.HasValue) {
                    existingProduct.Price = (double)product.Price;
                }

                if (product.Units.HasValue) {
                    existingProduct.Units = (int)product.Units;
                }

                if (product.Description != null) {
                    existingProduct.Description = product.Description;
                }

                if (product.Category != null) {
                    existingProduct.Category = product.Category;
                }

                db.SaveChanges();
                return Ok(existingProduct);
            }

            return Unauthorized();
        }

        [HttpDelete]
        [Authorize]
        public ActionResult DeleteProduct([FromQuery] Guid productId)
        {
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            if (userRole == "admin") {
                Product product = db.Products.Find(productId);
                db.Products.Remove(product);
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

        [HttpGet("results")]
        public ActionResult SearchProduct([FromQuery] SearchResource searchResource)
        {
            List<Product> products;
            if (searchResource.Name != null && searchResource.Category != null) {
                products = db.Products.Where(x => x.Name == searchResource.Name && x.Category == searchResource.Category).ToList();
            } else if (searchResource.Name != null) {
                products = db.Products.Where(x => x.Name == searchResource.Name).ToList();
            } else if (searchResource.Category != null) {
                products = db.Products.Where(x => x.Category == searchResource.Category).ToList();
            } else {
                products = new List<Product>();
            }

            return Ok(_mapper.Map<List<Product>, List<ProductResourceOut>>(products));
        }

        [HttpGet("recommendation")]
        [Authorize]
        public ActionResult RecommendProduct([FromQuery] SearchResource searchResource)
        {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;
            var userEMail = User.FindFirst(c=>c.Type==ClaimTypes.Email)?.Value;

            var boughtProducts = db.BoughtProducts.Where(x => x.User.Id.ToString() == userId).ToList();
            var categories = new List<string>();
            foreach (BoughtProduct bp in boughtProducts) {
                Product product = db.Products.Find(bp.ProductId);
                categories.Add(product.Category);
            }
            var mostCommonCategory = categories.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).Where(x => x != null).First();

            return Ok(mostCommonCategory);
        }
    }
}

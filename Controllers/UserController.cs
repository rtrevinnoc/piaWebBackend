using System.IdentityModel.Tokens.Jwt;
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
        public JWTResponse CreateToken(User user) {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: Configuration["JWT:ValidIssuer"],
                audience: Configuration["JWT:ValidAudience"],
                claims: new List<Claim>() {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.EMail),
                            new Claim(ClaimTypes.Role, user.Role.ToString())
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
                    return Ok(CreateToken(existingUser));
                } else {
                    return Unauthorized();
                }
            } else {
                var newUser = _mapper.Map<UserSign, User>(credentials);
                newUser.Password = protector.Protect(credentials.Password);
                db.Users.Add(newUser);
                db.SaveChanges();

                return Ok(CreateToken(newUser));
            }
        }


        [HttpGet]
        public ActionResult SignIn([FromQuery] UserLogIn credentials)
        {
            if (credentials is null)
            {
                return BadRequest();
            }

            var existingUser = db.Users.FirstOrDefault(x => x.UserName == credentials.UserName);

            if (protector.Unprotect(existingUser.Password) == credentials.Password) {
                return Ok(CreateToken(existingUser));
            }

            return Unauthorized();
        }

        [HttpGet("cart")]
        [Authorize]
        public ActionResult GetCart()
        {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            // if (userRole == "buyer") {
                var cart = db.BoughtProducts.Where(x => x.User.Id.ToString() == userId && x.OrderId == null).ToList();
                // return Ok(_mapper.Map<ICollection<Product>, ICollection<ProductResourceOut>>(cart));
                return Ok(cart);
            // }

            // return Unauthorized();
        }

        [HttpPut("cart")]
        [Authorize]
        public ActionResult UpdateCart([FromQuery] BuyResource buyResource)
        {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            // if (userRole == "buyer") {
                var user = db.Users.FirstOrDefault(x => x.Id.ToString() == userId);

                var product = db.Products.FirstOrDefault(x => x.Name == buyResource.ProductName);

                var cart = db.BoughtProducts.Where(x => x.User.Id.ToString() == userId && x.OrderId == null);

                var cartItem = cart.FirstOrDefault(x => x.ProductName == buyResource.ProductName);

                if (cartItem == null) {
                    var boughtProduct = new BoughtProduct();
                    boughtProduct.Id = Guid.NewGuid();

                    if (buyResource.Quantity >= product.Units) {
                        boughtProduct.Units = product.Units;
                        product.Units = 0;
                    } else {
                        product.Units = product.Units - buyResource.Quantity;
                        boughtProduct.Units = buyResource.Quantity;
                    }

                    boughtProduct.ProductName = product.Name;
                    boughtProduct.ProductId = product.Id;
                    db.BoughtProducts.Add(boughtProduct);
                    user.Cart.Add(boughtProduct);
                } else {

                    if (buyResource.Quantity >= 0) {
                        if (buyResource.Quantity >= product.Units) {
                            cartItem.Units = cartItem.Units + product.Units;
                            product.Units = 0;
                        } else {
                            product.Units = product.Units - buyResource.Quantity;
                            cartItem.Units = cartItem.Units + buyResource.Quantity;
                        }
                    } else {
                        if (-buyResource.Quantity >= cartItem.Units) {
                            product.Units = product.Units + cartItem.Units;
                            cartItem.Units = 0;
                        } else {
                            product.Units = product.Units - buyResource.Quantity;
                            cartItem.Units = cartItem.Units + buyResource.Quantity;
                        }
                    }


                }

                db.SaveChanges();

                // return Ok(_mapper.Map<ICollection<Product>, ICollection<ProductResourceOut>>(user.Cart));
                return Ok(cart);
            // }

            // return Unauthorized();
        }

        [HttpDelete("cart")]
        [Authorize]
        public ActionResult ClearCart()
        {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            // if (userRole == "buyer") {
                var cart = db.BoughtProducts.Where(x => x.User.Id.ToString() == userId && x.OrderId == null);

                foreach (BoughtProduct bp in cart.ToList()) {
                    Product product = db.Products.Find(bp.ProductId);
                    product.Units = product.Units + bp.Units;
                }

                cart.ExecuteDelete();
                db.SaveChanges();

                return Ok(cart);
            // }

            // return Unauthorized();
        }

        [HttpGet("cart/order")]
        [Authorize]
        public ActionResult PlaceOrder()
        {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            // if (userRole == "buyer") {
                var cart = db.BoughtProducts.Where(x => x.User.Id.ToString() == userId && x.OrderId == null).ToList();

                var order = new Order();
                order.Id = Guid.NewGuid();
                order.Date = DateTime.Now;
                order.Status = Entidades.Status.placed;
                order.Products = cart;

                double total = 0;
                foreach (BoughtProduct bp in cart) {
                    Product product = db.Products.Find(bp.ProductId);
                    bp.OrderId = order.Id;
                    total = total + (bp.Units * product.Price);
                }

                order.Total = total;
                db.Orders.Add(order);
                db.SaveChanges();

                // return Ok(_mapper.Map<ICollection<Product>, ICollection<ProductResourceOut>>(cart));
                return Ok(order);
            // }

            // return Unauthorized();
        }

        [HttpPut("cart/order/pay")]
        [Authorize]
        public ActionResult PayOrder([FromBody] PaymentResource paymentResource)
        {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value;

            // if (userRole == "buyer") {
                Order order = db.Orders.Find(paymentResource.OrderId);
                order.Status = Entidades.Status.paid;
                order.Address = paymentResource.Address;
                order.PaymentMethod = paymentResource.PaymentMethod;

                ClearCart();

                db.SaveChanges();

                // return Ok(_mapper.Map<ICollection<Product>, ICollection<ProductResourceOut>>(cart));
                return Ok(order);
            // }

            // return Unauthorized();
        }

        [HttpGet("orders")]
        [Authorize]
        public ActionResult GetOrders() {
            var userId = User.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;

            var orders = db.Orders.ToList();
            foreach (Order order in orders) {
                order.Products = db.BoughtProducts.Where(x => x.User.Id.ToString() == userId && x.OrderId == order.Id).ToList();
            }
            return Ok(orders);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moinitores.Entidades;
using Monitores.Entidades;

namespace Monitores.Controllers{
    
    [ApiController]
    [Route("api/Pedido")]

    public class ApplicationDbContext: DbContext{
        private readonly ApplicationDbContext dbContext;

        [HttpGet]
        public async Task<ActionResult<List<Pedido>>> Get(){
            return await dbContext.pedido.ToListAsync();
        }

        [HttpPost]
        public async  Task<ActionResult> Post(Pedido pedido ){
            var existeCarri= await dbContext.carrito.AnyAsync(X =>{X.id = pedido.id});

            if(!existeCarri){
                return BadRequest("No existe ningun pedido que halla estado en el carrito antes con ese id");
            }

            dbContext.Update(pedido);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(Pedido pedido, int id){
             var existeCarri= await dbContext.carrito.AnyAsync(X =>{X.id = pedido.id});

             if(!existeCarri){
                BadRequest("No existe el carrito con este id")
             }

             if(X.id != pedido.id){
                BadRequest("No coincide el id con ningun pedido echo");
             }

             dbContext.Update(pedido);
             await dbContext.SaveChangesAsync();
             return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Pedido pedido, int id){
            var existe = await dbContext.pedido.AnyAsync(X =>{X.id= id});

            if(!existe){
                BadRequest("No existe ningun pedido con ese id");
            }

            dbContext.Remove( new Pedido(){
                id=id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        } 

    }
}
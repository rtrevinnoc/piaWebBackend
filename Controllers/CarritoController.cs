using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitores.Entidades;

namespace Monitores.Controllers{
    
    [ApiController]
    [Route("api/Carrito")]

    public class CarritoController: DbContext{

        private readonly ApplicationDbContext dbContext;

        [HttpGet]
        public async Task<ActionResult<List<Carrito>>> Get(){

            return await dbContext.Carrito.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Carrito carrito){
            var existeProduc= await dbContext.productos.AnyAsync(X =>{X.id= carrito.id});

            if(!existeProduc){
            return BadRequest("No existe un producto asociado en nuestra tienda");
        }

        dbContext.Add(carrito);
        await dbContext.SaveChangesAsync();
        return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(Carrito carrito, int id){

             var existeProduc= await dbContext.productos.AnyAsync(X =>{X.id= carrito.id});

            if(!existeProduc){
            return BadRequest("No existe un producto asociado en nuestra tienda");
        }

        if(X.id != carrito.producto_id){
            return BadRequest("el producto no esta asociado a este carrito");
        }

        dbContext.Remove(carrito);
        await dbContext.SaveChangesAsync();
        return Ok();

        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Carrito carrito, int id){
             
             var existe= await dbContext.carrito.AnyAsync(X =>{X.id= id});

            if(!existe){
            return BadRequest("No existe ningun producto registrado con ese id");
            }

            dbContext.Remove(new Carrito() {
                id=id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }


    }
}
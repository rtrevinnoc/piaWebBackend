using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitores.Entidades;

namespace Monitores.Controllers
{

    [ApiController]
    [Route("api/Productos")]
    public class ProductosController: ControllerBase
{
    private readonly ApplicationDbContext dbContext;

    [HttpGet] //metodo Get
    public async Task<ActionResult<List<Productos>>> Get()
    {
        return await dbContext.productos.ToListAsync();
    }

    [HttpPost] //metodo post 
    public async Task<ActionResult> Post(Productos productos)
    {
        dbContext.Add(productos);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")] //metodo put 
    public async Task <ActionResult> Put(Productos productos, int id)
    {
        if(productos.id != id)
        {
            return BadRequest("El producto con el id dado no coincide con ninguno registrado");
        }

        dbContext.Update(productos);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")] // metodo delete 
    public async Task<ActionResult> Delete(Productos productos, int id)
    {
        var exist= await dbContext.productos.AnyAsync(x => x.id ==id);
            if (!exist)
        {
            return BadRequest("El alumno con el id proporcionado no se encuantra registrado");
        }

        dbContext.Remove(new Productos() {id = id});
         await dbContext.SaveChangesAsync();
        return Ok();
    }

    
}    
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitores.Entidades;

namespace Monitores.Controllers
{
    [ApiController]
    [Route("api/branches")]
    public class BranchesController : ControllerBase
    {
        private ApplicationDbContext db;

        public BranchesController(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        [HttpGet]
        public ActionResult<List<Branch>> Get() => db.Branches.Include(r => r.Company).ToList();


        [HttpPost]
        public ActionResult<Branch> Post(Branch monitor)
        {
            db.Branches.Add(monitor);
            db.SaveChanges();

            return monitor;
        }

        [HttpDelete]
        public ActionResult<Branch> Delete(Guid id)
        {
            Branch monitor = db.Branches.Find(id);
            db.Branches.Remove(monitor);
            db.SaveChanges();

            return monitor;
        }

        [HttpPut]
        public ActionResult<Branch> Put(Branch updatedBranch)
        {
            var oldBranch = db.Branches.Find(updatedBranch.BranchId);

            db.Branches.Entry(oldBranch).CurrentValues.SetValues(updatedBranch);
            db.SaveChanges();

            return updatedBranch;
        }

        public class addToRoomResource
        {
            public Guid roomId { get; set; }
            public Guid monitorId { get; set; }
        }

        [Route("addToRoom")]
        [HttpPost]
        public ActionResult<Branch> AddToRoom([FromBody] addToRoomResource addToRoomResource)
        {
            Branch branch = db.Branches.Find(addToRoomResource.monitorId);
            Company company = db.Companies.Find(addToRoomResource.roomId);

            company.Branches.Add(branch);
            branch.Company = company;

            db.Branches.Entry(branch).Reference(e => e.Company).IsModified = true;
            db.Companies.Entry(company).Collection(e => e.Branches).IsModified = true;

            db.SaveChanges();

            return branch;
        }
    }
}

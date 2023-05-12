using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monitores.Entidades;

namespace Monitores.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private ApplicationDbContext db;

        public CompaniesController(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        [HttpGet("hello")]
        [Authorize]
        public ActionResult<string> Get() => "hello";


        [HttpPost]
        public ActionResult<Company> Post(Company company)
        {
            db.Companies.Add(company);
            db.SaveChanges();

            return company;
        }

        public class claimMonitorResource
        {
            public Guid companyId { get; set; }
            public Guid branchId { get; set; }
        }

        [Route("claimMonitor")]
        [HttpPost]
        public ActionResult<Company> ClaimMonitor([FromBody] claimMonitorResource claimMonitorResource)
        {
            Company company = db.Companies.Find(claimMonitorResource.companyId);
            Branch branch = db.Branches.Find(claimMonitorResource.branchId);

            company.Branches.Add(branch);
            branch.Company = company;

            db.Branches.Entry(branch).Reference(e => e.Company).IsModified = true;
            db.Companies.Entry(company).Collection(e => e.Branches).IsModified = true;

            db.SaveChanges();

            return company;
        }

        [HttpDelete]
        public ActionResult<Company> Delete(Guid id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
            db.SaveChanges();

            return company;
        }

        [HttpPut]
        public ActionResult<Company> Put(Company updatedCompany)
        {
            var oldCompany = db.Companies.Find(updatedCompany.CompanyId);

            db.Companies.Entry(oldCompany).CurrentValues.SetValues(updatedCompany);
            db.SaveChanges();

            return updatedCompany;
        }
    }
}

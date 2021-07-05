using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using WingGateway.Classes;
using Microsoft.AspNetCore.Authorization;

namespace WingGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnapiController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly IConfiguration _config;
        public ReturnapiController(DBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/Returnapi
        [HttpPost]
        [Route("GetCountry")]
        public async Task<ActionResult<IEnumerable<tblCountryMaster>>> GettblCountryMaster()
        {
            return await _context.tblCountryMaster.ToListAsync();
        }

        // GET: api/Returnapi/5
        [HttpPost]
        [Route("GetCountry/{id}")]
        public async Task<ActionResult<tblCountryMaster>> GettblCountryMaster(int id)
        {
            var tblCountryMaster = await _context.tblCountryMaster.FindAsync(id);

            if (tblCountryMaster == null)
            {
                return NotFound();
            }

            return tblCountryMaster;
        }
        
        [HttpPost]
        [Route("GetState")]
        public async Task<ActionResult<IEnumerable<tblStateMaster>>> GettblStateMaster()
        {
            return await _context.tblStateMaster.ToListAsync();
        }

        // GET: api/Returnapi/5
        [HttpPost]
        [Route("GetState/{id}")]
        public async Task<ActionResult<tblStateMaster>> GettblStateMaster(int id)
        {
            var tblStateMaster = await _context.tblStateMaster.FindAsync(id);

            if (tblStateMaster == null)
            {
                return NotFound();
            }

            return tblStateMaster;
        }




        [HttpGet]
        [Route("CreateRoleClaims")]
        public async Task<ActionResult<bool>> CreateRoleClaims([FromServices] RoleManager<IdentityRole> RoleManager)
        {
            
            string[] roleNames = { "TC", "Employee", "Finance","Operation" };
            IdentityResult roleResult;
            List<Claim> AllClaim= ClaimStore.GetClaims();


            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    IdentityRole identityRole = new IdentityRole(roleName);
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(identityRole);
                    if (roleResult.Succeeded)
                    {
                        
                    }
                }
            }

            var identityRoleTc = RoleManager.Roles.Where(p => p.Name == "TC").FirstOrDefault();
            var AlreadyClaims = (await RoleManager.GetClaimsAsync(identityRoleTc)).Select(p => p.Type).ToList();
            var toBeInserted = AllClaim.Where(p => !AlreadyClaims.Contains(p.Type) && p.Type.IndexOf("Gateway_") >= -1).ToList();
            foreach (var t in toBeInserted)
            {
                var result = await RoleManager.AddClaimAsync(identityRoleTc, t);
            }

            var identityRoleEmployee = RoleManager.Roles.Where(p => p.Name == "Employee").FirstOrDefault();            
            var toBeInsertedEmp = AllClaim.Where(p => !AlreadyClaims.Contains(p.Type) && p.Type.IndexOf("Emp_") >= -1).ToList();
            foreach (var t in toBeInsertedEmp)
            {
                var result = await RoleManager.AddClaimAsync(identityRoleEmployee, t);
            }

            return true;
        }

        [Authorize]
        [Route("GetTree/{Id}")]
        public  ActionResult<Models.mdlTreeWraper> GetTree([FromServices] IConsProfile cons, string Id)
        {
            
            int Nid=cons.GetNId(Id);            
            return  cons.GetAllDownline(Nid);
            
            
        }


        //// PUT: api/Returnapi/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PuttblCountryMaster(int id, tblCountryMaster tblCountryMaster)
        //{
        //    if (id != tblCountryMaster.CountryId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(tblCountryMaster).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!tblCountryMasterExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Returnapi
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<tblCountryMaster>> PosttblCountryMaster(tblCountryMaster tblCountryMaster)
        //{
        //    _context.tblCountryMaster.Add(tblCountryMaster);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GettblCountryMaster", new { id = tblCountryMaster.CountryId }, tblCountryMaster);
        //}

        //// DELETE: api/Returnapi/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletetblCountryMaster(int id)
        //{
        //    var tblCountryMaster = await _context.tblCountryMaster.FindAsync(id);
        //    if (tblCountryMaster == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.tblCountryMaster.Remove(tblCountryMaster);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool tblCountryMasterExists(int id)
        //{
        //    return _context.tblCountryMaster.Any(e => e.CountryId == id);
        //}
    }
}

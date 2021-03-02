using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Database;

namespace WingGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnapiController : ControllerBase
    {
        private readonly DBContext _context;

        public ReturnapiController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Returnapi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tblCountryMaster>>> GettblCountryMaster()
        {
            return await _context.tblCountryMaster.ToListAsync();
        }

        // GET: api/Returnapi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<tblCountryMaster>> GettblCountryMaster(int id)
        {
            var tblCountryMaster = await _context.tblCountryMaster.FindAsync(id);

            if (tblCountryMaster == null)
            {
                return NotFound();
            }

            return tblCountryMaster;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<tblStateMaster>>> GettblStateMaster()
        {
            return await _context.tblStateMaster.ToListAsync();
        }

        // GET: api/Returnapi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<tblStateMaster>> GettblStateMaster(int id)
        {
            var tblStateMaster = await _context.tblStateMaster.FindAsync(id);

            if (tblStateMaster == null)
            {
                return NotFound();
            }

            return tblStateMaster;
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

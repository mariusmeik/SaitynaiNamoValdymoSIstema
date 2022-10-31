using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaitynaiNamoValdymoSIstema.DataDB;
using SaitynaiNamoValdymoSIstema.DTOs;

namespace SaitynaiNamoValdymoSIstema.Controllers
{
    [Route("api/Floors/{floorId}/[controller]")]
    [ApiController]
    public class FlatsController : ControllerBase
    {
        private readonly SaitynaiNamoValdymoSistemaDBContext _context;
        private readonly IMapper _mapper;
        public FlatsController(SaitynaiNamoValdymoSistemaDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet, Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetFlats(int floorId)
        {
            var floors = _context.Flats.Where(p => p.FloorId == floorId);
            if (floors.Count() == 0)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<List<Flat>>(floors.ToList()));
        }

        // GET: api/Flats/5
        [HttpGet("{id}"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetFlat(int id, int floorId)
        {
            var flat = await _context.Flats.FindAsync(id);

            if (flat == null || flat.FloorId != floorId)
            {
                return NotFound("flat with tis id not found, or searched on the wrong floor");
            }
            return Ok(_mapper.Map<FlatDTO>(flat));
        }

        // PUT: api/Flats/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutFlat(int id, Flat flat, int floorId)
        {
            if (id != flat.Id || flat.FloorId != floorId)
            {
                return BadRequest("id does not match");
            }
            try
            {
                Floor? floor = await _context.Floors.FirstOrDefaultAsync(p => p.Id == floorId);
                if (floor == null)
                {
                    return BadRequest("no floor with that id");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            _context.Entry(flat).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlatExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(flat.Id);
        }

        // POST: api/Flats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostFlat(FlatDTO flat, int floorId)
        {
            if (flat.FloorId != floorId)
            {
                return BadRequest("flat.floorId and floorId did not match");
            }
            try
            {
                Floor? floor = await _context.Floors.FirstOrDefaultAsync(p => p.Id == floorId);
                if (floor == null)
                {
                    return NotFound("no floor with that id");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            Flat f = _mapper.Map<Flat>(flat);

            _context.Flats.Add(f);
            await _context.SaveChangesAsync();

            return Ok(f.Id);
        }

        // DELETE: api/Flats/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFlat(int id, int floorId)
        {
            var flat = await _context.Flats.FindAsync(id);
            if (flat == null)
            {
                return NotFound();
            }
            if (flat.FloorId != floorId)
            {
                return BadRequest("floorId does not match flat.floorId, won't delete");
            }
            var people = await _context.People.FirstOrDefaultAsync(p => p.FlatId == id);
            if (people != null)
            {
                return BadRequest("Flat is not empty");
            }
            _context.Flats.Remove(flat);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlatExists(int id)
        {
            return _context.Flats.Any(e => e.Id == id);
        }
    }
}

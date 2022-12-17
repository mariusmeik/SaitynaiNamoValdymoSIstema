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
using SaitynaiNamoValdymoSIstema.DTOs.RequestExamples;
using Swashbuckle.AspNetCore.Filters;

namespace SaitynaiNamoValdymoSIstema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorsController : ControllerBase
    {
        private readonly SaitynaiNamoValdymoSistemaDBContext _context;
        private readonly IMapper _mapper;

        public FloorsController(SaitynaiNamoValdymoSistemaDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Floors
        [HttpGet, Authorize(Roles = "User,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Floor>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFloors()
        {
            List<Floor> floors = await _context.Floors.ToListAsync();
            return Ok(floors);
        }

        // GET: api/Floors/5
        [HttpGet("{id}"), Authorize(Roles = "User,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Floor))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetFloor(int id)
        {
            var floor = await _context.Floors.FindAsync(id);
            if (floor == null)
            {
                return NotFound();
            }
            return Ok(floor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="floor"></param>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     Put /Todo
        ///     { 
        ///        "Id" = 1,
        ///        "WhichFloor" = 1
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(Floor), typeof(FloorPutRequestExample))]
        public async Task<IActionResult> PutFloor(int id, Floor floor)
        {
            if (id != floor.Id)
            {
                return BadRequest();
            }

            _context.Entry(floor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FloorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(204);
        }

        // POST: api/Floors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostFloor(FloorDTO floor)
        {
            Floor newFloor = _mapper.Map<Floor>(floor);
            _context.Floors.Add(newFloor);
            await _context.SaveChangesAsync();

            return Ok(newFloor.Id);
        }

        // DELETE: api/Floors/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFloor(int id)
        {
            var floor = await _context.Floors.FindAsync(id);
            if (floor == null)
            {
                return NotFound();
            }
            var flat = await _context.Flats.FirstOrDefaultAsync(p => p.FloorId == id);
            if (flat != null)
            {
                return BadRequest("floor is not empty");
            }
            _context.Floors.Remove(floor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FloorExists(int id)
        {
            return _context.Floors.Any(e => e.Id == id);
        }
    }
}

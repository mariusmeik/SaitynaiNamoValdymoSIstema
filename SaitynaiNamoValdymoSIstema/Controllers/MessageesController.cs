using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Route("api/[controller]")]
    [ApiController]
    public class MessageesController : ControllerBase
    {
        private readonly SaitynaiNamoValdymoSistemaDBContext _context;
        private readonly IMapper _mapper;

        public MessageesController(SaitynaiNamoValdymoSistemaDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       // GET: api/Messagees
       [HttpGet, Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetMessagees()
        {
            return Ok(await _context.Messagees.ToListAsync());
        }

       // GET: api/Messagees/5
        [HttpGet("{id}"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetMessagee(int id)
        {
            var messagee = await _context.Messagees.FindAsync(id);

            if (messagee == null)
            {
                return NotFound();
            }

            return Ok(messagee);
        }

       // PUT: api/Messagees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> PutMessagee(int id, MessagePutDTO message)
        {
            Messagee messagee = new Messagee();
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var someClaim = claimsIdentity.Claims.ToList();
            int PersonId = int.Parse(someClaim[0].Value);
            if (messagee.PersonId == PersonId|| someClaim[3].Value=="Admin"){}
            else
            {
                return BadRequest();
            }
            messagee.PersonId = PersonId;
            messagee.TextMessage = message.TextMessage;
            if (id != messagee.Id)
            {
                return BadRequest();
            }
            try
            {
                Person? person = await _context.People.FirstOrDefaultAsync(p => p.Id == messagee.PersonId);
                if (person == null)
                {
                    throw new EntryPointNotFoundException("no floor with that id");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            _context.Entry(messagee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //POST: api/Messagees
       // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> PostMessagee(MessageDTO messagee)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var someClaim = claimsIdentity.Claims.ToList();
            int PersonId = int.Parse(someClaim[0].Value);
           

            try
            {
                Person? person = await _context.People.FirstOrDefaultAsync(p => p.Id == PersonId);
                if (person == null || person.IsApproved == false)
                {
                    throw new EntryPointNotFoundException("no person with that id or person is not allowed to post messages");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            Messagee m = _mapper.Map<Messagee>(messagee);
            m.PersonId = PersonId;
            _context.Messagees.Add(m);
            await _context.SaveChangesAsync();

            return Ok(m.Id);
        }

        //DELETE: api/Messagees/5
        [HttpDelete("{id}"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> DeleteMessagee(int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var someClaim = claimsIdentity.Claims.ToList();
            int PersonId = int.Parse(someClaim[0].Value);
           


            var messagee = await _context.Messagees.FindAsync(id);
            if (messagee == null)
            {
                return NotFound();
            }
            if (messagee.PersonId == PersonId || someClaim[3].Value == "Admin") { }
            else
            {
                return BadRequest();
            }

            _context.Messagees.Remove(messagee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageeExists(int id)
        {
            return _context.Messagees.Any(e => e.Id == id);
        }
    }
}

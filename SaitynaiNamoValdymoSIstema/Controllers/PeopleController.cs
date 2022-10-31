using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaitynaiNamoValdymoSIstema.DataDB;
using SaitynaiNamoValdymoSIstema.DTOs;
using XSystem.Security.Cryptography;

namespace SaitynaiNamoValdymoSIstema.Controllers
{
    [Route("api/Floors/{floorId}/Flats/{flatId}/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly SaitynaiNamoValdymoSistemaDBContext _context;
        private readonly IMapper _mapper;
        XSystem.Security.Cryptography.MD5CryptoServiceProvider md5 = new XSystem.Security.Cryptography.MD5CryptoServiceProvider();
        public PeopleController(SaitynaiNamoValdymoSistemaDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        private async Task<Person> GetPersonHelper(int id, int flatId, int floorId)
        {
            Person? person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
            Flat? flat = await _context.Flats.FirstOrDefaultAsync(p => p.Id == person.FlatId);
            Floor? floor = await _context.Floors.FirstOrDefaultAsync(p => p.Id == flat.FloorId);
            if (flat.Id != flatId || floor.Id != floorId)
            {
                throw new ArgumentException("person doesnot live in a given floor or flat");
            }
            _context.Entry(person).State = EntityState.Detached;
            return person;
        }
        // GET: api/People
        [HttpGet, Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetPeople(int flatId, int floorId)
        {
            var listOfFlats = await _context.Flats.Where(p => p.FloorId == floorId && p.Id == flatId).ToListAsync();
            var people1 = await _context.People.Where(p => listOfFlats.Select(p => p.Id).Contains(p.FlatId)).ToListAsync();
            var c = people1.Where(p => p.IsApproved == true).ToList();
            return Ok(c);
        }

        // GET: api/People/5
        [HttpGet("{id}"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetPerson(int id, int flatId, int floorId)
        {
            Person person = new();
            try
            {
                person = await GetPersonHelper(id, flatId, floorId);
            }
            catch (Exception ex)
            {
                NotFound(ex.Message);
            }
            if (person == null || person.IsApproved == false)
            {
                NotFound();
            }
            return Ok(person);
        }

        // PUT: api/People/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutPerson(int id, int flatId, int floorId, PersonDTO person)
        {
            Person personNew = new();
            try
            {
                personNew = await GetPersonHelper(id, flatId, floorId);
            }
            catch (Exception ex)
            {
                NotFound(ex.Message);
            }
            if (personNew == null)
            {
                NotFound();
            }

            CreatePasswordHash(person.Password, out byte[] passwordHash);
            personNew = _mapper.Map<Person>(person);
            personNew.Password = passwordHash;
            _context.Entry(personNew).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
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
        private void CreatePasswordHash(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                hmac.Key = Encoding.ASCII.GetBytes("123");
                //passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash)
        {
            var passwordSalt = Encoding.ASCII.GetBytes("123");
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        // POST: api/People
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> PostPerson(PersonDTO person, int flatId, int floorId)
        {
            Flat? f = await _context.Flats.FirstOrDefaultAsync(p => p.Id == flatId);
            if (f == null || f.FloorId != floorId || person.FlatId != flatId || person.FlatId != f.Id)
            {
                return BadRequest();
            }
            Person personN = new();
            personN = _mapper.Map<Person>(person);
            //hashing
            CreatePasswordHash(person.Password, out byte[] passwordHash);
            personN.Password = passwordHash;

            await _context.People.AddAsync(personN);
            await _context.SaveChangesAsync();

            return Ok(personN.Id);
        }

        // DELETE: api/People/5
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePerson(int id, int flatId, int floorId)
        {
            Person personNew = new();
            try
            {
                personNew = await GetPersonHelper(id, flatId, floorId);
            }
            catch (Exception ex)
            {
                NotFound(ex.Message);
            }
            if (personNew == null)
            {
                NotFound();
            }
            List<Messagee> messages = await _context.Messagees.Where(p => p.PersonId == id).ToListAsync();
            foreach (Messagee m in messages)
            {
                _context.Messagees.Remove(m);
            }
            await _context.SaveChangesAsync();
            _context.People.Remove(personNew);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}

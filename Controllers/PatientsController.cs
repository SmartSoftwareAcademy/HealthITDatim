using HealthITDatim.Data;
using HealthITDatim.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HealthITDatim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class PatientsController : ControllerBase
    {
        private readonly DatimData _context;
        public PatientsController(DatimData context)
        {
            _context=context;
        }
        // GET: api/<PatientsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> Get()
        {
            return await _context.Patient.OrderByDescending(x=>x.Id).Where(x=>x.delete_status==0).ToListAsync();
        }

        // GET api/<PatientsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> Get(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            if(patient == null)
            {
                return NotFound();
            }
            return patient;
        }

        // POST api/<PatientsController>
        [HttpPost]
        public async Task<ActionResult<Patient>> Post([FromBody] Patient patient)
        {
             _context.Add(patient);
            try
            {
                await _context.SaveChangesAsync();
            }catch(Exception ex)
            {
                ex.Message.ToString();
            }
            return CreatedAtAction("Get", new {Id=patient.Id},patient);
        }

        // PUT api/<PatientsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Patient>> Put(int id, [FromBody] Patient patient)
        {
            if(id != patient.Id)
            {
                return BadRequest();
            }
            _context.Entry(patient).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }catch(Exception ex)
            {
                ex.Message.ToString();
            }
            return NoContent();
        }

        // DELETE api/<PatientsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> Delete(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            patient.delete_status = 1;
            _context.Entry(patient).State= EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }catch(Exception ex)
            {
                ex.Message.ToString();
            }
            return NoContent();
        }
    }
}

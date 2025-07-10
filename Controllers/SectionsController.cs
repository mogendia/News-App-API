
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using NewsApp.DTOs;
using NewsApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly NewsDbContext _context;

        public SectionsController(NewsDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SectionDto>>> GetSections()
        {
            return await _context.Sections
                .Select(s => new SectionDto { Id = s.Id, Name = s.Name })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SectionDto>> GetSection(int id)
        {
           return await _context.Sections
                .Select(s => new SectionDto { Id = s.Id, Name = s.Name })
                .FirstOrDefaultAsync(s => s.Id == id);
        }

     
        [HttpPost]
        public async Task<ActionResult<SectionDto>> PostSection(CreateSectionDto createSectionDto)
        {
            var section = new Section
            {
                Name = createSectionDto.Name
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            var sectionDto = new SectionDto
            {
                Id = section.Id,
                Name = section.Name
            };

            return CreatedAtAction("GetSection", new { id = section.Id }, sectionDto);
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSection(int id, CreateSectionDto createSectionDto)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }

            section.Name = createSectionDto.Name;
            _context.Entry(section).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SectionExists(id))
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

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null)
            {
                return NotFound();
            }

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SectionExists(int id)
        {
            return _context.Sections.Any(e => e.Id == id);
        }
    }
}


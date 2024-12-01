using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDiaryAPI.Data;
using WebDiaryAPI.Models;

namespace WebDiaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaryEntriesController : ControllerBase
    {
        //get access to diary entry model
        private readonly ApplicationDbContext _context;

        public DiaryEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //make it async so that the main thread is not blocked and request is handled more efficiently
        [HttpGet]
        //GET: api/DiaryEntries
        public async Task<ActionResult<IEnumerable<DiaryEntry>>> GetDiaryEntries()
        {
            return await _context.DiaryEntries.ToListAsync();
        }

        //get a specific diary entry from DB
        //GET: api/DiaryEntries/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DiaryEntry>> GetDiaryEntryById(int id)
        {
            //get data from db and filter it
            var diaryEntry = await _context.DiaryEntries.FindAsync(id);//when async varition of the meth is used, the return type must be a Task<>

            if (diaryEntry == null) return NotFound(); //because ActionREsult wraps both diaryentry or action result

            return diaryEntry;
        }

        //create an item in db
        //POST:api/DiaryEntries
        [HttpPost]
        public async Task<ActionResult<DiaryEntry>> PostDiaryEntry(DiaryEntry diaryEntry)
        {
            //check if id is 0 so that a new ID will be generated while inserting the record in db
            diaryEntry.Id = 0;

            _context.DiaryEntries.Add(diaryEntry);

            await _context.SaveChangesAsync();

            var resourceUrl = Url.Action(nameof(GetDiaryEntryById), new { id = diaryEntry.Id });
            return Created(resourceUrl, diaryEntry);
        }

        //update an item in db
        //PUT : /api/DiaryEntries/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiaryEntry(int id, [FromBody] DiaryEntry diaryEntry)
        {
            //[FromBody] - parameter binding. Binds the params from req.body to DiaryEntry obj
            //handled by Entity framework- deserialize JSON and into obj
            if (id != diaryEntry.Id) return BadRequest(); //send 400 response if IDs mismatch

            //update the db with the data for given id
            //marks diaryentry as modified in db context. Tells entity framework to update this in db 
            _context.Entry(diaryEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!DiaryEntryExists(id)) return NotFound();
                else throw;
            }
            return NoContent();//code 204 - update successful
        }

        //helps to throw a more precise error message
        private bool DiaryEntryExists(int id)
        {
            return _context.DiaryEntries.Any(e => e.Id == id);
        }

    }
}

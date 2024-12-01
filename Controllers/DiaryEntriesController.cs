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
            return Created(resourceUrl,diaryEntry);
        }
    }
}

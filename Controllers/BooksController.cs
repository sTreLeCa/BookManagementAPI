namespace BookManagementAPI.Controllers
{
    using BookManagementAPI.Interfaces;
    using BookManagementAPI.Models;
    using Microsoft.AspNetCore.Mvc;
    using MongoDB.Bson;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET /api/book
        [HttpGet]
        public async Task<IActionResult> GetBooksByPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.GetBooksByPage(page, pageSize);
            return Ok(books);
        }

        // GET /api/book/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
            {
                return BadRequest(new { message = "Invalid ID format" });
            }

            var book = await _bookService.Get(objectId);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        // POST /api/book
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Books book)
        {
            try
            {
                var result = await _bookService.Create(book);
                if (!result.success)
                {
                    return BadRequest(new { message = result.message });
                }
                return CreatedAtAction(nameof(Get), new { id = book.Id.ToString() }, book);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/book/many
        [HttpPost("many")]
        public async Task<IActionResult> CreateMany([FromBody] List<Books> books)
        {
            try
            {
                var warnings = await _bookService.CreateMany(books);

                if (warnings.Any())
                {
                    return BadRequest(new { message = "Some books were not added due to duplicates.", warnings });
                }

                return CreatedAtAction(nameof(Get), new { id = books[0].Id.ToString() }, books);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/book/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] Books book)
        {
            try
            {
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                {
                    return BadRequest(new { message = "Invalid ID format" });
                }

                book.Id = objectId;
                await _bookService.Update(objectId, book);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/book/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete([FromRoute] string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out ObjectId objectId))
                {
                    return BadRequest(new { message = "Invalid ID format" });
                }

                await _bookService.SoftDelete(objectId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/book
        [HttpDelete]
        public async Task<IActionResult> SoftDeleteMany([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { message = "No valid IDs provided." });
            }

            try
            {
                // Convert string IDs to ObjectIds with validation
                var objectIds = new List<ObjectId>();
                foreach (var id in ids)
                {
                    if (!ObjectId.TryParse(id, out ObjectId objectId))
                    {
                        return BadRequest(new { message = $"Invalid ID format: {id}" });
                    }
                    objectIds.Add(objectId);
                }

                await _bookService.SoftDeleteMany(objectIds);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
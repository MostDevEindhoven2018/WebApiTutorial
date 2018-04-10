using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoApi.Controllers
{
    /// <summary>
    /// Defines an empty controller class. In the next sections, methods are added to implement the API.
    /// The constructor uses Dependency Injection to inject the database context (TodoContext) into the controller.
    /// The database context is used in each of the CRUD methods in the controller.
    /// The constructor adds an item to the in-memory database if one doesn't exist.
    /// </summary>
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Returns all items from the database.
        /// </summary>
        /// <returns>List of toDo items.</returns>
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        /// <summary>
        /// Returns the toDo item with the specific id.
        /// </summary>
        /// <param name="id">The id of the item. long</param>
        /// <returns>A toDo item.</returns>
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item is null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        /// <summary>
        /// Creates a new toDo item to the database.
        /// </summary>
        /// <param name="item">The new toDo item. toDo</param>
        /// <returns>
        /// Returns a 201 response.HTTP 201 is the standard response for an HTTP POST method
        /// that creates a new resource on the server. Adds a Location header to the response. 
        /// The Location header specifies the URI of the newly created to-do item.
        /// Uses the "GetTodo" named route to create the URL. The "GetTodo" named route is defined in GetById
        /// </returns>
        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item is null)
            {
                return BadRequest();
            }

            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }

        /// <summary>
        /// Updates a specified toDo item with the given properties.
        /// </summary>
        /// <param name="id">The id of the item. long</param>
        /// <param name="item">The updates toDo item.</param>
        /// <returns> 
        /// The response is 204 (No Content). According to the HTTP spec, 
        /// a PUT request requires the client to send the entire updated entity, not just the deltas. 
        /// To support partial updates, use HTTP PATCH.
        /// </returns>
        [HttpPut("{id}")]
        public IActionResult Update (long id, [FromBody] TodoItem item)
        {
            if (item is null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);

            if (todo is null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();

            return new NoContentResult();
        }

        /// <summary>
        /// Deletes the toDo item with the specific id.
        /// </summary>
        /// <param name="id">The id of the item. long</param>
        /// <returns>The Delete response is 204 (No Content).</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var toDo = _context.TodoItems.FirstOrDefault(t => t.Id == id);

            if (toDo is null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(toDo);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}

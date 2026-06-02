using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using Tasks.Data;
using Tasks.Models;

namespace Tasks.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            // Recuperamos las tareas y las enviamos en formato JSON al tablero Kanban
            var tasks = _context.WorkItems.Select(t => new {
                id = t.Id,
                title = t.Title,
                status = (int)t.Status // Convertimos el Enum a número para que el Javascript lo entienda
            }).ToList();

            return Json(tasks);
        }

        [HttpPost]
        public IActionResult AddTask(string title, string assignedUser, string taskType, DateTime? startDate, DateTime? endDate)
        {
            if (!string.IsNullOrEmpty(title))
            {
                var newItem = new WorkItem
                {
                    Title = title,
                    Status = WorkItemStatus.ToDo, // Por defecto al tablero "To Do" (o Backlog)
                    TaskType = taskType,
                    StartDate = startDate,
                    EndDate = endDate,
                    Priority = "Media" // Prioridad por defecto
                    // Nota: 'assignedUser' por ahora es texto, cuando integres bien Identity, guardaremos el 'AssignedUserId' real.
                };

                _context.WorkItems.Add(newItem);
                _context.SaveChanges();
            }

            return Ok();
        }
    }
}

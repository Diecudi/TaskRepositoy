using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Tasks.Data;
using Tasks.Models;

namespace Tasks.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> AddTask(string title, string assignedUser, string taskType, DateTime? startDate, DateTime? endDate)
        {
            if (!string.IsNullOrEmpty(title))
            {
                // Identificamos al usuario que tiene la sesión abierta
                var currentUser = await _userManager.GetUserAsync(User);

                var newItem = new WorkItem
                {
                    Title = title,
                    Description = "", // Obligatorio en BD, lo enviamos vacío por defecto
                    Status = WorkItemStatus.ToDo, 
                    TaskType = taskType ?? "Tarea",
                    StartDate = startDate,
                    EndDate = endDate,
                    Priority = "Media",
                    AssignedUserId = currentUser.Id // <- ¡Aquí asociamos la tarea al usuario!
                };

                _context.WorkItems.Add(newItem);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}

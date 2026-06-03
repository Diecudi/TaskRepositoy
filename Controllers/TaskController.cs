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
        public IActionResult GetProjects()
        {
            var projects = _context.Projects.Select(p => new { id = p.Id, name = p.Name }).ToList();
            return Json(projects);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(string name, string description)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var project = new Project { Name = name, Description = description ?? "" };
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return Ok(new { id = project.Id });
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult GetSprints(int projectId)
        {
            var sprints = _context.Sprints.Where(s => s.ProjectId == projectId)
                .Select(s => new { 
                    id = s.Id, 
                    name = s.Name, 
                    startDate = s.StartDate.ToString("dd/MM/yyyy"), 
                    endDate = s.EndDate.ToString("dd/MM/yyyy") 
                }).ToList();
            return Json(sprints);
        }

        [HttpPost]
        public async Task<IActionResult> AddSprint(string name, DateTime startDate, DateTime endDate, int projectId)
        {
            if (!string.IsNullOrEmpty(name) && projectId > 0)
            {
                _context.Sprints.Add(new Sprint { Name = name, StartDate = startDate, EndDate = endDate, ProjectId = projectId });
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult GetTasks(int projectId)
        {
            var tasks = _context.WorkItems.Where(t => t.ProjectId == projectId).Select(t => new {
                id = t.Id,
                title = t.Title,
                status = (int)t.Status,
                sprintId = t.SprintId,
                startDate = t.StartDate.HasValue ? t.StartDate.Value.ToString("dd/MM/yyyy") : "",
                endDate = t.EndDate.HasValue ? t.EndDate.Value.ToString("dd/MM/yyyy") : "",
                assignedUser = _context.Users.Where(u => u.Id == t.AssignedUserId).Select(u => u.FullName).FirstOrDefault() ?? "Sin Asignar"
            }).ToList();

            return Json(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string title, string assignedUser, string taskType, DateTime? startDate, DateTime? endDate, int projectId, int? sprintId)
        {
            if (!string.IsNullOrEmpty(title) && projectId > 0)
            {
                // Identificamos al usuario que tiene la sesión abierta
                var currentUser = await _userManager.GetUserAsync(User);

                var newItem = new WorkItem
                {
                    Title = title,
                    Description = "", // Obligatorio en BD, lo enviamos vacío por defecto
                    Status = WorkItemStatus.ToDo, 
                    TaskType = taskType ?? "Tarea",
                    StartDate = startDate ?? DateTime.Now,
                    EndDate = endDate ?? DateTime.Now.AddDays(1),
                    CreatedAt = DateTime.Now,
                    Priority = "Media",
                    AssignedUserId = currentUser.Id,
                    ProjectId = projectId,
                    SprintId = sprintId == 0 ? null : sprintId
                };

                _context.WorkItems.Add(newItem);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskStatus(int id, int status, int? sprintId)
        {
            var task = await _context.WorkItems.FindAsync(id);
            if (task != null)
            {
                task.Status = (WorkItemStatus)status;
                if (sprintId.HasValue) {
                    task.SprintId = sprintId.Value == 0 ? null : sprintId.Value;
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}

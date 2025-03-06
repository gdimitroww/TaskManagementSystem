using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string statusFilter = "All", string sortOrder = "dueDate_asc")
        {
            var viewModel = new TaskListViewModel
            {
                StatusFilter = statusFilter,
                SortOrder = sortOrder
            };

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Tasks.Include(t => t.User).AsQueryable();

            // Show all tasks to all users
            // The "Edit" and "Delete" actions will still be restricted based on ownership

            // Apply status filter
            if (statusFilter != "All")
            {
                if (Enum.TryParse<Models.TaskStatus>(statusFilter, out var status))
                {
                    query = query.Where(t => t.Status == status);
                }
            }

            // Apply sorting
            query = sortOrder switch
            {
                "title_asc" => query.OrderBy(t => t.Title),
                "title_desc" => query.OrderByDescending(t => t.Title),
                "dueDate_asc" => query.OrderBy(t => t.DueDate),
                "dueDate_desc" => query.OrderByDescending(t => t.DueDate),
                "status_asc" => query.OrderBy(t => t.Status),
                "status_desc" => query.OrderByDescending(t => t.Status),
                "priority_asc" => query.OrderBy(t => t.Priority),
                "priority_desc" => query.OrderByDescending(t => t.Priority),
                _ => query.OrderBy(t => t.DueDate),
            };

            viewModel.Tasks = await query.ToListAsync();
            return View(viewModel);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to view this task
            if (!User.IsInRole("Admin") && task.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            var model = new TaskItemViewModel
            {
                DueDate = DateTime.Today.AddDays(1)
            };
            return View(model);
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                var task = new TaskItem
                {
                    Title = model.Title,
                    Description = model.Description,
                    DueDate = model.DueDate,
                    Status = model.Status,
                    Priority = model.Priority,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to edit this task
            if (!User.IsInRole("Admin") && task.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            var model = new TaskItemViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status,
                Priority = task.Priority,
                UserId = task.UserId
            };

            return View(model);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItemViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var task = await _context.Tasks.FindAsync(id);
                    if (task == null)
                    {
                        return NotFound();
                    }

                    // Check if the user is authorized to edit this task
                    if (!User.IsInRole("Admin") && task.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                    {
                        return Forbid();
                    }

                    task.Title = model.Title;
                    task.Description = model.Description;
                    task.DueDate = model.DueDate;
                    task.Status = model.Status;
                    task.Priority = model.Priority;
                    task.UpdatedAt = DateTime.UtcNow;

                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Tasks/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (taskItem == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Check if the current user owns the task or is an admin
            if (taskItem.UserId != userId && !User.IsInRole("Admin"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View(taskItem);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            
            if (taskItem == null)
            {
                return NotFound();
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Check if the current user owns the task or is an admin
            if (taskItem.UserId != userId && !User.IsInRole("Admin"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            _context.Tasks.Remove(taskItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/ChangeStatus/5?status=InProgress
        public async Task<IActionResult> ChangeStatus(int? id, Models.TaskStatus status)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to change the status of this task
            if (!User.IsInRole("Admin") && task.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            
            _context.Update(task);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
} 
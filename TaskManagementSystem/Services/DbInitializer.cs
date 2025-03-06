using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using Polly;

namespace TaskManagementSystem.Services
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Use Polly to retry the database connection
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        10, // Number of retries
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                        }
                    );

                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Test the connection
                    await _context.Database.CanConnectAsync();
                    _logger.LogInformation("Successfully connected to the database");
                });

                // Ensure database is created and migrations are applied
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database created and migrations applied successfully");

                // Check if roles exist, create them if not
                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(roleName));
                        _logger.LogInformation($"Created role: {roleName}");
                    }
                }

                // Create admin user if it doesn't exist
                var adminEmail = "admin@example.com";
                var adminUser = await _userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                        _logger.LogInformation($"Created admin user: {adminEmail}");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError($"Error creating admin user: {errors}");
                    }
                }

                // Create a sample user if none exists
                var userEmail = "user@example.com";
                var normalUser = await _userManager.FindByEmailAsync(userEmail);

                if (normalUser == null)
                {
                    normalUser = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(normalUser, "User123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(normalUser, "User");
                        _logger.LogInformation($"Created regular user: {userEmail}");

                        // Create sample tasks for the user
                        var tasks = new List<TaskItem>
                        {
                            new TaskItem
                            {
                                Title = "TEST TASK - Documentation",
                                Description = "This is a test task to ensure tasks are displayed correctly in the list view.",
                                DueDate = DateTime.Now.AddDays(7),
                                Status = Models.TaskStatus.ToDo,
                                Priority = Models.TaskPriority.Medium,
                                UserId = normalUser.Id,
                                CreatedAt = DateTime.UtcNow
                            },
                            new TaskItem
                            {
                                Title = "Schedule team meeting",
                                Description = "Coordinate with team members to find suitable time for weekly sync-up.",
                                DueDate = DateTime.Now.AddDays(2),
                                Status = Models.TaskStatus.InProgress,
                                Priority = Models.TaskPriority.High,
                                UserId = normalUser.Id,
                                CreatedAt = DateTime.UtcNow
                            },
                            new TaskItem
                            {
                                Title = "Research new technologies",
                                Description = "Research emerging technologies that could improve our development workflow.",
                                DueDate = DateTime.Now.AddDays(14),
                                Status = Models.TaskStatus.ToDo,
                                Priority = Models.TaskPriority.Low,
                                UserId = normalUser.Id,
                                CreatedAt = DateTime.UtcNow
                            }
                        };

                        _context.Tasks.AddRange(tasks);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Added sample tasks for the regular user");
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError($"Error creating regular user: {errors}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
            }
        }
    }
} 
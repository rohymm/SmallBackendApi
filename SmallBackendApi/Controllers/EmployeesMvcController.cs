using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmallBackendApi.Models;
using SmallBackendApi.ViewModels;

namespace SmallBackendApi.Controllers
{
    // MVC controller: returns Razor views (HTML pages), not JSON
    public class EmployeesMvcController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeesMvcController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /EmployeesMvc  (list page)
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .ToListAsync();

            return View(employees);
        }

        // GET: /EmployeesMvc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Tasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: /EmployeesMvc/Create  (show the empty form)
        public async Task<IActionResult> Create()
        {
            await LoadDepartmentsAsync();
            return View(new CreateEmployeeViewModel());
        }

        // POST: /EmployeesMvc/Create  (the form is submitted here)
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            // Validation failed: show the same form again with error messages
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync();
                return View(model);
            }

            // Map the ViewModel to the entity
            var employee = new Employee
            {
                Name = model.Name,
                Salary = model.Salary,
                DepartmentId = model.DepartmentId,
                HireDate = model.HireDate
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /EmployeesMvc/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var model = new EditEmployeeViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Salary = employee.Salary,
                DepartmentId = employee.DepartmentId
            };

            await LoadDepartmentsAsync();
            return View(model);
        }

        // POST: /EmployeesMvc/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditEmployeeViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync();
                return View(model);
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = model.Name;
            employee.Salary = model.Salary;
            employee.DepartmentId = model.DepartmentId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /EmployeesMvc/Delete/5  (confirmation page)
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: /EmployeesMvc/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Fill the department dropdown list used by Create and Edit
        private async Task LoadDepartmentsAsync()
        {
            var departments = await _context.Departments.AsNoTracking().ToListAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
        }
    }
}
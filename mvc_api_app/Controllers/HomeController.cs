using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_api_app.Models;

namespace mvc_api_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentsContext _studentsContext;

        public HomeController(StudentsContext studentsContext)
        {
            _studentsContext = studentsContext;
        }

        // READ 
        public async Task<IActionResult> Index()
        {
            var students = await _studentsContext.Students.ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _studentsContext.Students
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Role,Status")] Student student)
        {
            if (ModelState.IsValid)
            {
                var existingIds = await _studentsContext.Students.Select(s => s.Id).OrderBy(id => id).ToListAsync();

                int freeId = 1;
                foreach (var id in existingIds)
                {
                    if (id == freeId)
                    {
                        freeId++;
                    }
                    else if (id > freeId)
                    {
                        break;
                    }
                }

                student.Id = freeId;

                using (var transaction = await _studentsContext.Database.BeginTransactionAsync())
                {
                    await _studentsContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students ON;");

                    _studentsContext.Students.Add(student);
                    await _studentsContext.SaveChangesAsync();

                    await _studentsContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students OFF;");
                    await transaction.CommitAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // UPDATE
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _studentsContext.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Role,Status")] Student student)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _studentsContext.Update(student);
                    await _studentsContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _studentsContext.Students
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentsContext.Students.FindAsync(id);
            if (student != null)
            {
                _studentsContext.Students.Remove(student);
                await _studentsContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _studentsContext.Students.Any(e => e.Id == id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BestMovies.Models;
using BestMovies.Areas.Admin.Models;

namespace BestMovies.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Admin/Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Admin/Movies/Create
        public IActionResult Create()
        {
            var vm = new CreateMovieViewModel() { Genres = _context.Genres.OrderBy(x => x.Name).ToList() };
            return View(vm);
        }

        // POST: Admin/Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovieViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Movie movie = new Movie()
                {
                    Name = vm.Name,
                    Year = vm.Year.Value,
                    Rating = vm.Rating.Value,
                    ImageUrl = vm.ImageUrl,
                    ImdbId = vm.ImdbId,
                    Genres = new List<Genre>()
                };

                foreach (int genreId in vm.SelectedGenres)
                {
                    movie.Genres.Add(_context.Genres.Find(genreId));
                }
                _context.Movies.Add(movie);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            vm.Genres = _context.Genres.OrderBy(x => x.Name).ToList();
            return View(vm);
        }

        // GET: Admin/Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id.Value);
            if (movie == null)
            {
                return NotFound();
            }

            var vm = new EditMovieViewModel()
            {
                Id = movie.Id,
                Name = movie.Name,
                ImageUrl = movie.ImageUrl,
                ImdbId = movie.ImdbId,
                Rating = movie.Rating,
                Year = movie.Year,
                Genres = await _context.Genres.ToListAsync(),
                SelectedGenres = movie.Genres.Select(x => x.Id).ToArray()
            };

            return View(vm);
        }

        // POST: Admin/Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, EditMovieViewModel vm)
        {

            if (id != vm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var movie = await _context.Movies.Include(x => x.Genres).FirstOrDefaultAsync(x => x.Id == id.Value);
                if(movie == null)
                {
                    return NotFound();
                }

                movie.Name = vm.Name;
                movie.ImageUrl = vm.ImageUrl;
                movie.ImdbId = vm.ImdbId;
                movie.Rating = vm.Rating.Value;
                movie.Year = vm.Year.Value;
                movie.Genres.Clear();
                var selectedGenres = _context.Genres.Where(x => vm.SelectedGenres.Contains(x.Id));
                movie.Genres.AddRange(selectedGenres);

                // todo: Veritabanındaki orjinal veriyi çek
                // üzerindeki alanları güncelle
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            vm.Genres = await _context.Genres.ToListAsync();
            return View(vm);
        }

        // GET: Admin/Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Admin/Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}

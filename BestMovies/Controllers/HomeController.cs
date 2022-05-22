using BestMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BestMovies.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var vm = new HomeViewModel()
            {
                Movies = _db.Movies
                    .Include(x => x.Genres)
                    .ToList(),
                Genres = _db.Genres.OrderBy(x => x.Name).Select(x => new GenreWithMovieCountViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    MovieCount = x.Movies.Count()
                }).ToList()
            };
            return View(vm);
        }

        public IActionResult IndexWithGenre(string genreName)
        {
            Genre genre = _db.Genres.Include(g => g.Movies).ThenInclude(m => m.Genres).FirstOrDefault(g => g.Name == genreName);

            if (genre == null) return NotFound();
            var vm = new HomeViewModel()
            {
                Movies = genre.Movies,
                Genres = _db.Genres.OrderBy(x => x.Name).Select(x => new GenreWithMovieCountViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    MovieCount = x.Movies.Count()
                }).ToList(),
                GenreId = genre.Id
            };
            return View("Index", vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

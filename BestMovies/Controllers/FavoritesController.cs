using BestMovies.Dtos;
using BestMovies.Models;
using BestMovies.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BestMovies.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly FavoriteService _favoriteService;

        public FavoritesController(ApplicationDbContext db, FavoriteService favoriteService)
        {
            _db = db;
            _favoriteService = favoriteService;
        }

        public IActionResult Index()
        {
            List<int> favs = _favoriteService.GetFavList();
            List<Movie> movies = _db.Movies.Where(x => favs.Contains(x.Id)).ToList();
            return View(movies);
        }
        
        [HttpPost]
        public IActionResult Toggle(int movieId)
        {
            List<int> favs = _favoriteService.GetFavList();
            bool favorited;

            if (favs.Contains(movieId))
            {
                favs.Remove(movieId);
                favorited = false;
            }
            else
            {
                favs.Add(movieId);
                favorited = true;
            }

            _favoriteService.SaveFavList(favs);

            return Json(new FavoriteToggleResult() { Favorited = favorited });
        }

    }
}


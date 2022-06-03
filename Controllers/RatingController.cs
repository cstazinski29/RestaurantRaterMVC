using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantRaterMVC.Data;
using RestaurantRaterMVC.Models;
using RestaurantRaterMVC.Models.Rating;

namespace RestaurantRaterMVC.Controllers
{
    public class RatingController : Controller
    {
        private RestaurantDbContext _context;
        public RatingController(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<RatingListItem> ratings = _context.Ratings
            .Select(r => new RatingListItem()
            {
                RestaurantName = r.Restaurant.Name,
                Score = r.Score
            });

            return View(ratings);
        }

        public async Task<IActionResult> Restaurant(int id)
        {
            IEnumerable<RatingListItem> ratings = await _context.Ratings
                .Where(r => r.RestaurantId == id)
                .Select(r => new RatingListItem()
                {
                    RestaurantName = r.Restaurant.Name,
                    Score = r.Score
                }).ToListAsync();

                Restaurant restaurant = await _context.Restaurants.FindAsync(id);
                ViewBag.RestaurantName = restaurant.Name;

                return View(ratings);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RatingCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Rating rating = new Rating()
            {
                RestaurantId = model.RestaurantId,
                Score = model.Score
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
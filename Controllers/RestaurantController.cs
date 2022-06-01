using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantRaterMVC.Data;
using RestaurantRaterMVC.Models.Restaurant;

namespace RestaurantRaterMVC.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ILogger<RestaurantController> _logger;
        private RestaurantDbContext _context;
        public RestaurantController(ILogger<RestaurantController> logger, RestaurantDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index() {
            List<RestaurantListItem> restaurants = await _context.Restaurants
            .Include(r => r.Ratings)
            .Select(r => new RestaurantListItem()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Score = r.Score
                }).ToListAsync();
            return View(restaurants);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RestaurantCreate model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.errorMessage = ModelState.FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage;
                return View(model);
            }
            Restaurant restaurant = new Restaurant()
            {
                Name = model.Name,
                Location = model.Location
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
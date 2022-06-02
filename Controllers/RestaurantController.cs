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

        [ActionName("Details")]
        public async Task<IActionResult> Restaurant(int id)
        {
            Restaurant restaurant = await _context.Restaurants
                .Include(r => r.Ratings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant == null)
                return RedirectToAction(nameof(Index));

                RestaurantDetail restaurantDetail = new RestaurantDetail()
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Location = restaurant.Location,
                    Score = restaurant.Score
                };

                return View(restaurantDetail);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Restaurant restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return RedirectToAction(nameof(Index));
            
            RestaurantEdit restaurantEdit = new RestaurantEdit()
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Location = restaurant.Location
            };

            return View(restaurantEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, RestaurantEdit model)
        {
            if (!ModelState.IsValid)
            {
                return View(ModelState);
            }

            Restaurant restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
                return RedirectToAction(nameof(Index));

            restaurant.Name = model.Name;
            restaurant.Location = model.Location;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = restaurant.Id });
        }
    }
}
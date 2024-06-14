using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        public DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var currentUser = _context.Users.FirstOrDefault(x => x.Login == user.Login && x.Password == user.Password);
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(user);
            }

            // Assuming User.Id is the primary key
            HttpContext.Session.SetInt32("UserId", currentUser.Id);
            if (currentUser.Role == "Admin")
                HttpContext.Session.SetString("Role", "Admin");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult LoginAsGuest()
        {            
            int guestUserId = 0;

            HttpContext.Session.SetInt32("UserId", guestUserId);

            return RedirectToAction("Index", "Home");
        }
    }
}

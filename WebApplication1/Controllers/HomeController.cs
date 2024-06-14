using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public HomeController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;           
        }

        public IActionResult Index()
        {            
            var urls = _context.URLs.ToList();
            
            bool isLoggedIn = HttpContext.Session.GetInt32("UserId") > 0;
            bool isAdmin = HttpContext.Session.GetString("Role") == "Admin";
            ViewBag.IsLoggedIn = isLoggedIn;
            ViewBag.IsAdmin = isAdmin;
            
            if (isLoggedIn)
            {
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            }

            return View(urls);
        }


        [HttpPost]
        public IActionResult AddUrl(string longUrl)
        {
            var existingUrl = _context.URLs.FirstOrDefault(u => u.LongUrl == longUrl);
            if (existingUrl != null)
            {
                return BadRequest("URL already exists.");
            }

            string shortUrl = GenerateShortUrl();

            var newUrl = new URL
            {
                LongUrl = longUrl,
                ShortUrl = shortUrl,
                CreateDate = DateTime.Now,
                CreatedBy = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"))
            };

            _context.URLs.Add(newUrl);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteUrl(int id)
        {
            var urlToDelete = _context.URLs.FirstOrDefault(u => u.Id == id && u.CreatedBy == Convert.ToInt32(HttpContext.Session.GetInt32("UserId")));

            if (urlToDelete != null)
            {
                _context.URLs.Remove(urlToDelete);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public IActionResult RedirectToLongUrl(string shortUrl)
        {
            var url = _context.URLs.FirstOrDefault(u => u.ShortUrl == shortUrl);

            if (url != null)
            {                               
                return Json(new { longUrl = url.LongUrl });
            }
            else
            {
                return NotFound();
            }
        }

        
        private string GenerateShortUrl()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] shortIdChars = new char[6];
            int shortUrlLength = 6;
            for (int i = 0; i < shortUrlLength; i++)
            {
                shortIdChars[i] = chars[random.Next(chars.Length)];
            }
            string shortUrl = new string(shortIdChars);
            
            while (_context.URLs.Any(u => u.ShortUrl == shortUrl))
            {
                for (int i = 0; i < shortUrlLength; i++)
                {
                    shortIdChars[i] = chars[random.Next(chars.Length)];
                }
                shortUrl = new string(shortIdChars);
            }

            return shortUrl;
        }

        public IActionResult Details(int id)
        {            
            var url = _context.URLs.FirstOrDefault(u => u.Id == id);

            if (url == null)
            {
                return NotFound(); 
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == url.CreatedBy);

            if (user == null)
            {
                return NotFound(); 
            }

            ViewBag.CreatedByUserName = user.Login; 

            return View(url); 
        }

        public IActionResult About()
        {
            // Load content from about.txt file
            string aboutContent = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "about.txt"));

            // Assuming IsAdmin is set based on some logic (e.g., role check)
            bool isAdmin = HttpContext.Session.GetString("Role") == "Admin";            
            ViewBag.IsAdmin = isAdmin;
            ViewBag.AboutContent = aboutContent;

            return View();
        }

        [HttpPost]
        public IActionResult About(string aboutContent)
        {
            // Check if user is an admin
            if (HttpContext.Session.GetString("Role") != "Admin")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            // Save the edited content to about.txt file
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "about.txt");
            System.IO.File.WriteAllText(filePath, aboutContent);

            // Redirect to the About page after editing
            return RedirectToAction("About");
        }
    }
}
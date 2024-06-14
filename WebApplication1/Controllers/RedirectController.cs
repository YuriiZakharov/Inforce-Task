using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly DataContext _context;

        public RedirectController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{shortUrl}")]
        public async Task<IActionResult> MoveTo(string shortUrl)
        {           
            var url = await _context.URLs.FirstOrDefaultAsync(u => u.ShortUrl == shortUrl);

            if (url != null)
            {
                return Redirect(url.LongUrl);
            }
            else
            {
                return NotFound();
            }
        }
    }
}

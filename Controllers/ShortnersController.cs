using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortner;

namespace UrlShortner.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortnersController : ControllerBase
    {
        private readonly ShortnerContext _context;

        public ShortnersController(ShortnerContext context)
        {
            _context = context;
        }

        // GET: api/Shortners
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shortner>>> GetUrlShortner()
        {
            return await _context.UrlShortner.ToListAsync();
        }

        // GET: api/Shortners/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Shortner>> GetShortner(int id)
        {
            var shortner = await _context.UrlShortner.FindAsync(id);

            if (shortner == null)
            {
                return NotFound();
            }

            return Redirect(shortner.oUrl);
        }

        // PUT: api/Shortners/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShortner(int id, Shortner shortner)
        {
            if (id != shortner.ID)
            {
                return BadRequest();
            }

            _context.Entry(shortner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShortnerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Shortners
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Shortner>> PostShortner(Shortner shortner)
        {
            Uri urlcheck;
            WebRequest req;
            //bool UrlTest = true;
            if (shortner.oUrl.Contains("http://") || shortner.oUrl.Contains("https://"))
            {
                urlcheck = new Uri(shortner.oUrl);
            }
            else
            {
                urlcheck = new Uri("http://" + shortner.oUrl);
                shortner.oUrl = "http://" + shortner.oUrl;
            }
             
             req = WebRequest.Create(urlcheck);
            try
            {
                var res = req.GetResponse();
            }
            catch (Exception)
            {
                return NotFound();
            }
            

            
           
                var val =  _context.UrlShortner.Where(l=>l.oUrl==shortner.oUrl).FirstOrDefaultAsync();
            
            if (val.Result==null)
            {
                var rowid = _context.UrlShortner.OrderByDescending(p => p.ID).FirstOrDefault();
                if (rowid == null)
                {
                    shortner.simUrl = "https://localhost:85/api/shortners/" + 1;
                    _context.UrlShortner.Add(shortner);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetShortner", new { id = shortner.ID }, shortner);
                }
                int shrt = rowid.ID+1;
                shortner.simUrl = "https://localhost:85/api/shortners/" + shrt;
                _context.UrlShortner.Add(shortner);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetShortner", new { id = shortner.ID }, shortner);
            }
            else
            {
                var smurl = await _context.UrlShortner.FindAsync(val.Result.ID);
                return smurl;
            }
        }

        // DELETE: api/Shortners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShortner(int id)
        {
            var shortner = await _context.UrlShortner.FindAsync(id);
            if (shortner == null)
            {
                return NotFound();
            }

            _context.UrlShortner.Remove(shortner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShortnerExists(int id)
        {
            return _context.UrlShortner.Any(e => e.ID == id);
        }
    }
}

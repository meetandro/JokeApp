using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JokeApp.Data;
using JokeApp.Models;

namespace JokeApp.Controllers;

public class JokesController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    // GET: Jokes
    public async Task<IActionResult> Index()
    {
        return View(await _context.Jokes.ToListAsync());
    }

    // GET: Jokes/Search
    public IActionResult Search()
    {
        return View();
    }

    // POST: Jokes/Search
    [HttpPost]
    public async Task<IActionResult> Search(string phrase)
    {
        var results = _context.Jokes
            .Where(j => j.JokeAnswer.Contains(phrase) || j.JokeQuestion.Contains(phrase))
            .ToListAsync();

        return View("Index", await results);
    }

    // GET: Jokes/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (joke == null)
        {
            return NotFound();
        }

        return View(joke);
    }

    // GET: Jokes/Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Jokes/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,JokeQuestion,JokeAnswer,Author")] Joke joke)
    {
        if (ModelState.IsValid)
        {
            _context.Add(joke);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(joke);
    }

    // GET: Jokes/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes.FindAsync(id);
        if (joke == null)
        {
            return NotFound();
        }
        return View(joke);
    }

    // POST: Jokes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,JokeQuestion,JokeAnswer,Author")] Joke joke)
    {
        if (id != joke.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(joke);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JokeExists(joke.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(joke);
    }

    // GET: Jokes/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (joke == null)
        {
            return NotFound();
        }

        return View(joke);
    }

    // POST: Jokes/Delete/5
    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var joke = await _context.Jokes.FindAsync(id);
        if (joke != null)
        {
            _context.Jokes.Remove(joke);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool JokeExists(int id)
    {
        return _context.Jokes.Any(e => e.Id == id);
    }
}

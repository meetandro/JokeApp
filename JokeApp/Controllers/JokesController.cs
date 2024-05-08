using JokeApp.Data;
using JokeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JokeApp.Controllers;

public class JokesController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var jokes = await _context.Jokes.ToListAsync();
        return View(jokes);
    }

    public IActionResult Search()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Search(string phrase)
    {
        var matchingResults = await _context.Jokes
            .Where(j => j.JokeQuestion.Contains(phrase) || j.JokeAnswer.Contains(phrase))
            .ToListAsync();
        return View(nameof(List), matchingResults);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes.FindAsync(id);
        if (joke is null)
        {
            return NotFound();
        }
        return View(joke);
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Joke joke)
    {
        if (ModelState.IsValid)
        {
            _context.Add(joke);
            await _context.SaveChangesAsync();
            return RedirectToAction();
        }
        return View(joke);
    }

    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes.FindAsync(id);
        if (joke is null)
        {
            return NotFound();
        }
        return View(joke);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Joke joke)
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
                if (await _context.Jokes.FindAsync(id) is null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(List));
        }
        return View(joke);
    }

    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var joke = await _context.Jokes.FindAsync(id);
        if (joke is null)
        {
            return NotFound();
        }
        return View(joke);
    }

    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var joke = await _context.Jokes.FindAsync(id);
        if (joke is not null)
        {
            _context.Jokes.Remove(joke);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(List));
    }
}

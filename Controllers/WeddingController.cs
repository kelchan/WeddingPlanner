using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;


public class WeddingController : Controller
{
    

    // _context is just a variable name, can be called anything (e.g. DATABASE, db, _db, etc)
    private WeddingPlannerContext _context;

    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("UUID");
        }
    }

    private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }

    // here we can "inject" our context service into the constructor
    public WeddingController( WeddingPlannerContext context )
    {
        _context = context;
    }

    [ HttpGet( "/wedding/dashboard" ) ]
    public IActionResult Dashboard()
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        List<Wedding> AllWeddings = _context.Weddings
        .Include( wedding => wedding.WeddingCreator )
        .Include( wedding => wedding.Guests )
        .ToList();

        return View( "Dashboard", AllWeddings );
    }

    [ HttpGet( "/wedding/new" ) ]
    public IActionResult New()
    {
        int? userId = HttpContext.Session.GetInt32("UUID");
        if (userId == null)
        {
            return RedirectToAction("Index", "User");
        }

        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        return View( "New" );
    }

    [ HttpPost( "/posts/create" ) ]
    public IActionResult Create( Wedding newWedding )
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        if (ModelState.IsValid == false) {
            // by not defaulting the return of View() in New, we can invoke the New() function & not have to re-write code
            // return View("New");

            return New();
        }

        if (uid != null)
        {
            newWedding.UserId = (int)uid;
        }

        _context.Weddings.Add( newWedding );
        _context.SaveChanges();

        return RedirectToAction( "Dashboard" );
    }

    [ HttpGet( "/weddings/{weddingId}" ) ]
    public IActionResult ViewWedding( int weddingId )
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        // since firstordefault can return a single post, or null. our variable needs to be a nullable datatype
        Wedding? wedding = _context.Weddings
            .Include( p => p.WeddingCreator )
            .Include( p => p.Guests )
            // .ThenInclude is used for including something on the
            // object that was JUST included (for this example, our likes list)
            .ThenInclude( p => p.User )
            .FirstOrDefault( p => p.WeddingId == weddingId );
        
        // to get rid of "object might be null" warnings, write a conditional that checks for it & returns if it's null
        if ( wedding == null )
        {
            return RedirectToAction( "Dashboard" );
        }
        
        return View( "ViewWedding", wedding );
    }

    [ HttpPost( "/weddings/{deletedWeddingId}/delete" ) ]
    public IActionResult Delete( int deletedWeddingId )
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        Wedding? weddingToBeDeleted = _context.Weddings.FirstOrDefault( wedding => wedding.WeddingId == deletedWeddingId );

        if (weddingToBeDeleted != null)
        {
            if(weddingToBeDeleted.UserId == uid)
            {
                _context.Weddings.Remove( weddingToBeDeleted );
                _context.SaveChanges();
            }
        }

        return RedirectToAction( "Dashboard" );
    }

    [ HttpPost( "/weddings/{weddingId}/rsvp" ) ]
    public IActionResult Rsvp( int weddingId )
    {
        if(!loggedIn || uid == null)
        {
            return RedirectToAction("Index", "User");
        }

        Attend? existingAttend = _context.Attends.FirstOrDefault( rsvp => rsvp.WeddingId == weddingId && rsvp.UserId == uid );

        if ( existingAttend == null )
        {
            Attend newAttend = new Attend(){
                WeddingId = weddingId,
                UserId = (int)uid
            };
            _context.Attends.Add( newAttend );
        }
        else
        {
            _context.Remove( existingAttend );
        }


        _context.SaveChanges();
        return RedirectToAction( "Dashboard" );
    }

}
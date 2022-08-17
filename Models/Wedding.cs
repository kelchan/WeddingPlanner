#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models;

public class Wedding
{
    [ Key ]
    [ Display( Name = "" ) ]
    public int WeddingId { get; set; }

    [ Required( ErrorMessage = "is required" ) ]
    [Display(Name = "Wedder One")]
    public string WedderOne { get; set; }

    [ Required( ErrorMessage = "is required" ) ]
    [ Display( Name = "Wedder Two" ) ]
    public string WedderTwo { get; set; }

    [ Required( ErrorMessage = "is required" ) ]
    [ Display( Name = "Date" ) ]
    [ DataType( DataType.Date ) ]
    [ FutureDate ]
    public DateTime Date { get; set; }

    [ Required( ErrorMessage = "is required" ) ]
    [ Display( Name = "Wedding Address" ) ]
    public string WeddingAddress { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


    public int UserId { get; set; }
    public User? WeddingCreator { get; set; }
    public List<Attend> Guests { get; set; } = new List<Attend>();
    
}
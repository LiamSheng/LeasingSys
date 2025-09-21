using System;
using Microsoft.EntityFrameworkCore;
using LeasingSys_API.Models;

namespace LeasingSys_API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Leasing> Leasing { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Leasing>().HasData(
            new Leasing
            {
                Id = 1,
                Name = "Royal Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa3.jpg",
                Occupancy = 4,
                Rate = 200,
                Sqft = 550,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Leasing
            {
                Id = 2,
                Name = "Premium Pool Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa1.jpg",
                Occupancy = 4,
                Rate = 300,
                Sqft = 550,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Leasing
            {
                Id = 3,
                Name = "Luxury Pool Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa4.jpg",
                Occupancy = 4,
                Rate = 400,
                Sqft = 750,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Leasing
            {
                Id = 4,
                Name = "Diamond Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa5.jpg",
                Occupancy = 4,
                Rate = 550,
                Sqft = 900,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Leasing
            {
                Id = 5,
                Name = "Diamond Pool Villa",
                Details =
                    "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://dotnetmastery.com/bluevillaimages/villa2.jpg",
                Occupancy = 4,
                Rate = 600,
                Sqft = 1100,
                Amenity = "",
                CreatedDate = DateTime.Now
            });
    }
}
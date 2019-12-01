using BandAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.DbContexts
{
    public class BandAlbumContext : DbContext
    {
        public BandAlbumContext(DbContextOptions<BandAlbumContext> options) :
            base (options)
        {
        }

        public DbSet<Band> Bands { get; set; }
        public DbSet<Album> Albums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Band>().HasData(new Band()
            {
                Id = Guid.Parse("6b1eea43-5597-45a6-bdea-e68c60564247"),
                Name = "Metallica",
                Founded = new DateTime(1980, 1, 1),
                MainGenre = "Heavy Metal"
            },
            new Band {
                Id = Guid.Parse("a052a63d-fa53-44d5-a197-83089818a676"),
                Name = "Guns N Roses",
                Founded = new DateTime(1985, 2, 1),
                MainGenre = "Rock"
            },
            new Band
            {
                Id = Guid.Parse("cb554ed6-8fa7-4b8d-8d90-55cc6a3e0074"),
                Name = "ABBA",
                Founded = new DateTime(1965, 7, 1),
                MainGenre = "Disco"
            },
            new Band
            {
                Id = Guid.Parse("8e2f0a16-4c09-44c7-ba56-8dc62dfd792d"),
                Name = "Oasis",
                Founded = new DateTime(1991, 12, 1),
                MainGenre = "Alternative"
            },
            new Band
            {
                Id = Guid.Parse("cab51058-0996-4221-ba63-b841004e89dd"),
                Name = "A-ha",
                Founded = new DateTime(1981, 6, 1),
                MainGenre = "Pop"
            });

            modelBuilder.Entity<Album>().HasData(
                new Album
                {
                    Id = Guid.Parse("dc4ccabe-29aa-42c4-9f80-18caea50adf5"),
                    Title = "Master Of Puppets",
                    Description = "One of the best heavy metal albums ever",
                    BandId = Guid.Parse("6b1eea43-5597-45a6-bdea-e68c60564247")
                },
                new Album
                {
                    Id = Guid.Parse("e5b6e8bf-5956-4329-a1b3-b1d48eea33ad"),
                    Title = "Appetite for Destruction",
                    Description = "Amazing Rock album with raw sound",
                    BandId = Guid.Parse("a052a63d-fa53-44d5-a197-83089818a676")
                },
                new Album
                {
                    Id = Guid.Parse("380c545c-9665-4043-baf2-34a3edefd373"),
                    Title = "Waterloo",
                    Description = "Very groovy album",
                    BandId = Guid.Parse("cb554ed6-8fa7-4b8d-8d90-55cc6a3e0074")
                },
                new Album
                {
                    Id = Guid.Parse("0e9a4ab5-4ae6-4ca3-ae7b-5f813e022527"),
                    Title = "Be Here Now",
                    Description = "Arguably one of the best albums by Oasis",
                    BandId = Guid.Parse("8e2f0a16-4c09-44c7-ba56-8dc62dfd792d")
                },
                new Album
                {
                    Id = Guid.Parse("8d2744ff-1134-4f36-a300-043febdc64b8"),
                    Title = "Hunting Hight and Low",
                    Description = "Awesome Debut album by A-Ha",
                    BandId = Guid.Parse("cab51058-0996-4221-ba63-b841004e89dd")
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}

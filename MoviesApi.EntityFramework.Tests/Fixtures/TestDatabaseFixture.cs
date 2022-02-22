using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain.Entities;
using System;

namespace MoviesApi.EntityFramework.Tests.Fixtures
{
    public class TestDatabaseFixture
    {
        private const string ConnectionString = @"Server=localhost,11455;Database=MoviesDb;User Id=sa;Password=Pass123!";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        
                        context.AddRange(
                            new Movie(new Guid ("15C02618-C2B4-4443-A92C-8EA3115F2F57"), "The Matrix", "The original Matrix."),
                            new Movie(new Guid ("14FC4492-D0B5-4B85-B522-7B9195116C55"), "The Matrix Reloaded", "The sequel."));

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public MoviesContext CreateContext()
            => new(new DbContextOptionsBuilder<MoviesContext>()
                    .UseSqlServer(ConnectionString)
                    .Options);
    }
}

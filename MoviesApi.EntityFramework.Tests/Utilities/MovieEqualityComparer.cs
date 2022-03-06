using MoviesApi.Domain.Entities;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MoviesApi.EntityFramework.Tests.Utilities
{
    public class MovieEqualityComparer : IEqualityComparer<Movie>
    {
        public bool Equals(Movie? x, Movie? y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
                
            return x.Id == y.Id
                && x.Name == y.Name
                && x.Description == y.Description;
        }

        public int GetHashCode([DisallowNull] Movie obj)
        {
            return obj.Id.GetHashCode() ^ obj.Name.GetHashCode() ^ obj.Description.GetHashCode();
        }
    }
}

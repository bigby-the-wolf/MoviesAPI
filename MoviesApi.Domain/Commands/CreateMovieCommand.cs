using MoviesAPI.CQS;
using MoviesApi.Domain.Entities;

namespace MoviesApi.Domain.Commands;

public record CreateMovieCommand(Movie Movie) : ICommand;

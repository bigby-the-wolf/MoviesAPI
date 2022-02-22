﻿using MoviesApi.Domain.Commands;
using MoviesAPI.CQS;

namespace MoviesApi.EntityFramework.CommandHandlers
{
    public class CreateMovieCommandHandler : ICommandHandler<CreateMovieCommand>
    {
        private readonly MoviesContext _context;

        public CreateMovieCommandHandler(MoviesContext context)
        {
            _context = context;
        }

        public void Handle(CreateMovieCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
﻿using MediatR;
using Persistance;
using Application.Core;

namespace Application.Activities
{
    public class Delete
    {
       public class Command:IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
           // public Activity Activity{ get; set; }    
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
            _context= context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity=await _context.Activities.FindAsync(request.Id);
                if(activity == null)
                {
                    return null;
                }
                _context.Activities.Remove(activity);
               // _context.Activities.Remove(request.Activity);  
              var result=await  _context.SaveChangesAsync()>0;
                if(!result)
                {
                    return Result<Unit>.Failure("Failed to delete the activity");
                }
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Domain;
using Persistance;
using Application.Core;
using AutoMapper;
using FluentValidation;

namespace Application.Activities
{
    public class Edit
    {
        public class Command:IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        } 
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper mapper;
            public Handler(DataContext context,IMapper  mapper)
            {
                _context = context;
                this.mapper = mapper;
            }

            public MappingProfiles Profile { get; }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken) 
            {
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                if (activity == null) return null;
                mapper.Map(request.Activity,activity);
               
              //  
               // activity.Title = request.Activity.Title ??= "Updated Future Activity 10";
                var result=await _context.SaveChangesAsync()>0;
                if (!result)
                {
                    return Result<Unit>.Failure("Failed to Edit the Activity");
                }
                
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}

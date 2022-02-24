using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistance;
using AutoMapper.QueryableExtensions;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<ActivityDto>>> { }
        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly ILogger _logger;
            private readonly IMapper mapper;

            public Handler(DataContext context,ILogger<List> logger,IMapper mapper)
            {
                _context = context;
                _logger = logger;
                this.mapper = mapper;
            }
            public  async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    for(var i=0;i<10;i++)
                    {
                       cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(1000,cancellationToken);
                        _logger.LogInformation($"Task {i} is completed");
                    }

                }catch(Exception ex) when(ex is TaskCanceledException)
                {
                    _logger.LogInformation("Task is cancelled");
                }
                var activities = await _context.Activities
                    .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                //Egar loading
                //.Include(a=>a.Attendees)
                //.ThenInclude(x=>x.AppUser)

                //Projections
               // var activitiesResponse=mapper.Map<List<ActivityDto>>(activities);
                return Result<List<ActivityDto>>.Success(activities);
            }
        }
    }
}

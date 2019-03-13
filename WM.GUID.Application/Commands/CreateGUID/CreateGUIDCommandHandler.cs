using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using WM.GUID.Application.Exceptions;
using WM.GUID.Domain;
using WM.GUID.Application.Commands.CreateGUID;
using WM.GUID.Persistence;
using WM.GUID.Application.Queries.ReadGUID;
using Microsoft.Extensions.Caching.Distributed;

namespace WM.Application.GUIDs.Commands.CreateGUID
{
    public class CreateGUIDCommandHandler : IRequestHandler<CreateGUIDCommand, GuidDTO>
    {
        private readonly WMDbContext _context;
        private readonly IDistributedCache _cache;

        public CreateGUIDCommandHandler(WMDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<GuidDTO> Handle(CreateGUIDCommand request, CancellationToken cancellationToken)
        {
            //create rich domain object
            var entity = new GuidMetadata(request.Id, request.Expire, request.User);

            //add to database and save
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //create cache
            try
            {
                var cacheKey = entity.Id;
                var cacheMetadata = entity.Expire + "|" + entity.User;
                await _cache.SetStringAsync(cacheKey, cacheMetadata, cancellationToken);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //map to dto and return
            return GuidDTO.Create(entity);               
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WM.GUID.Application.Queries.ReadGUID;
using WM.GUID.Domain;
using WM.GUID.Persistence;

namespace WM.GUID.Application.Commands.UpdateGUID
{
    public class UpdateGUIDCommandHandler : IRequestHandler<UpdateGUIDCommand, GuidDTO>
    {
        private readonly WMDbContext _context;
        private readonly IDistributedCache _distributedCache;

        public UpdateGUIDCommandHandler(WMDbContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
        }

        public async Task<GuidDTO> Handle(UpdateGUIDCommand request, CancellationToken cancellationToken)
        {
            //create rich domain object, bypass null or empty name validation
            var entity = new GuidMetadata(request.Id, request.Expire, request.User ?? "N/A", request.IsDeleted);

            //modify database and save changes
            if (request.Expire != null) _context.Entry(entity).Property("Expire").IsModified = true;
            if (request.User != null) _context.Entry(entity).Property("User").IsModified = true;
            if (request.IsDeleted != null) _context.Entry(entity).Property("IsDeleted").IsModified = true;
            await _context.SaveChangesAsync(cancellationToken);

            //get existing cache
            var cacheKey = request.Id;
            var cache = _distributedCache.GetString(cacheKey); 
            
            //update existing cache
            string cacheMetadata;
            if (cache != null)
            {
                var data = cache.Split("|");
                var expire = data[0].ToString();
                var user = data[1].ToString();
                cacheMetadata = string.Concat(entity.Expire != null ? entity.Expire.ToString() : expire, "|"
                                            , entity.User != null ? entity.User : user);

                await _distributedCache.SetStringAsync(cacheKey, cacheMetadata, cancellationToken);
            }

            //map to dto and return
            return GuidDTO.Create(entity);
        }
    }
}

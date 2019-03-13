//using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
using WM.GUID.Application.Exceptions;
using WM.GUID.Domain;
using WM.GUID.Persistence;

namespace WM.GUID.Application.Queries.ReadGUID
{
    public class GetGUIDQueryHandler : IRequestHandler<GetGUIDQuery, GuidDTO>
    {

        private readonly WMDbContext _context;
        //private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;

        public GetGUIDQueryHandler(WMDbContext context, IDistributedCache distributedCache)
        {
            _context = context;
            //_mapper = mapper;
            _distributedCache = distributedCache;
        }

        public async Task<GuidDTO> Handle(GetGUIDQuery request, CancellationToken cancellationToken)
        {
            GuidMetadata entity;
            var cacheKey = request.Id;
            var cacheMetadata = _distributedCache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cacheMetadata))
            {
                //parse metadata and create rich domain object
                string[] metadata = cacheMetadata.Split('|');
                entity = new GuidMetadata(request.Id, Convert.ToInt64(metadata[0]), metadata[1]);
            }
            else
            {
                //get entity from database
                try
                {
                    entity = await _context.GUIDs.SingleAsync(c => c.Id == request.Id, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw ex;

                }

                //update cache
                cacheMetadata = entity.Expire + "|" + entity.User;              
                _distributedCache.SetString(cacheKey, cacheMetadata);
            }

            //check expired
            DateTime dateTime = DateTime.UtcNow;
            DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime.ToLocalTime());
            if (dateTimeOffset.ToUnixTimeSeconds() > entity.Expire)
                throw new ExpiredException();

            return GuidDTO.Create(entity);
        }
    }
}

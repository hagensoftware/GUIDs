
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WM.GUID.Persistence;

namespace WM.GUID.Application.Commands.DeleteGUID
{
    public class DeleteGUIDCommandHandler : IRequestHandler<DeleteGUIDCommand>
    {
        private readonly WMDbContext _context;
        private readonly IDistributedCache _distributedCache;

        public DeleteGUIDCommandHandler(WMDbContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
        }

        public async Task<Unit> Handle(DeleteGUIDCommand request, CancellationToken cancellationToken)
        {
            //soft delete from database
            var entity = await _context.GUIDs.SingleAsync(c => c.Id == request.Id, cancellationToken);
            entity.IsDeleted = true;
            _context.Entry(entity).Property("IsDeleted").IsModified = true;
            await _context.SaveChangesAsync(cancellationToken);

            //delete cache
            var cacheKey = request.Id;
            await _distributedCache.RemoveAsync(cacheKey, cancellationToken);

            //return void
            return Unit.Value;
        }
    }
}

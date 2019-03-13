using MediatR;

namespace WM.GUID.Application.Queries.ReadGUID
{
    public class GetGUIDQuery : IRequest<GuidDTO>
    {
        public string Id { get; set; }
    }
}


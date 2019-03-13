using MediatR;
using WM.GUID.Application.Queries.ReadGUID;

namespace WM.GUID.Application.Commands.CreateGUID
{
    public class CreateGUIDCommand : IRequest<GuidDTO>
    {
        //public CreateGUIDCommand(string guid, long? expire, string user)
        //{
        //    Id = guid;
        //    Expire = expire;
        //    User = user;
        //}

        public string Id { get; set; }

        public long? Expire { get; set; }

        public string User { get; set; }
    }
}

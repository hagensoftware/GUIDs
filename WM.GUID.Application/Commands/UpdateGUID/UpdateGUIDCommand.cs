using MediatR;
using WM.GUID.Application.Queries.ReadGUID;

namespace WM.GUID.Application.Commands.UpdateGUID
{
    public class UpdateGUIDCommand : IRequest<GuidDTO>
    {
        //public UpdateGUIDCommand(string id, long? expire, string user, bool isDeleted)
        //{
        //    Id = id;
        //    Expire = expire;
        //    User = user;
        //    IsDeleted = IsDeleted;
        //}

        public string Id { get; set; }

        public long? Expire { get; set; }

        public string User { get; set; }

        public bool? IsDeleted { get; set; }
    }

}

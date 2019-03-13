using MediatR;

namespace WM.GUID.Application.Commands.DeleteGUID
{
    public class DeleteGUIDCommand : IRequest
    { 
        //public DeleteGUIDCommand(string id)
        //{
        //    Id = id;
        //}

        public string Id { get; set; }
    }

}

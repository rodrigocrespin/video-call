using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoCall.Application;
using VideoCall.Application.Models;

namespace VideoCall.Api.Controllers
{
    [Route("api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IVideoCallService service;

        public RoomsController(IVideoCallService service)
        {
            this.service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(RoomModel[]), StatusCodes.Status200OK)]
        public Task<RoomModel[]> Get()
        {
            return service.GetAllRooms();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoomModel), StatusCodes.Status200OK)]
        public Task<RoomModel> GetSingle(string id)
        {
            return service.GetRoom(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RoomModel), StatusCodes.Status200OK)]
        public Task<RoomModel> Post([FromBody] CreateRoomParameters parameters)
        {
            return service.CreateRoom(parameters.Name, parameters.MaxParticipants);
        }
    }

    public record CreateRoomParameters(string Name, int? MaxParticipants);
}

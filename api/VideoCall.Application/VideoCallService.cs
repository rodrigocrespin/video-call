using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Base;
using Twilio.Clients;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;
using Twilio.Rest.Video.V1.Room;
using VideoCall.Application.Models;
using ParticipantStatus = Twilio.Rest.Video.V1.Room.ParticipantResource.StatusEnum;

namespace VideoCall.Application
{
    public interface IVideoCallService
    {
        string GetJwt(string? identity = null);
        Task<RoomModel> CreateRoom(string name, int? maxParticipants = null);
        Task<RoomModel> GetRoom(string id);
        Task<RoomModel[]> GetAllRooms();
    }

    public class VideoCallService : IVideoCallService
    {
        private readonly TwilioConfig config;
        private readonly IRoomMapper roomMapper;
        private readonly ILogger logger;

        public VideoCallService(IOptionsSnapshot<TwilioConfig> configAccessor, IRoomMapper roomMapper, ILogger<VideoCallService> logger)
        {
            this.roomMapper = roomMapper;
            this.logger = logger;
            config = configAccessor.Value ?? throw new Exception("Twilio config is required");
        }

        public string GetJwt(string? identity = null)
            => new Token(config.AccountSid,
                config.ApiKey,
                config.ApiSecret,
                identity ?? Guid.NewGuid().ToString(),
                grants: new HashSet<IGrant> { new VideoGrant() }).ToJwt();

        public async Task<RoomModel> CreateRoom(string name, int? maxParticipants = null)
        {
            var options = new CreateRoomOptions { UniqueName = name, MaxParticipants = maxParticipants };
            var room = await RoomResource.CreateAsync(options, GetClient());
            logger.LogInformation("Room id {id} created with name {name}", room.Sid, name);
            return roomMapper.Map(room);
        }

        public async Task<RoomModel> GetRoom(string id)
        {
            var room = await RoomResource.FetchAsync(id, GetClient());
            return roomMapper.Map(room, await ParticipantsTask(room.Sid));
        }

        public async Task<RoomModel[]> GetAllRooms()
        {
            var rooms = await RoomResource.ReadAsync(new ReadRoomOptions(), GetClient());
            var tasks = rooms.Select(room => GetRooms(room, ParticipantsTask(room.Sid)));
            return await Task.WhenAll(tasks);
        }

        private ITwilioRestClient GetClient() =>
            new TwilioRestClient(config.ApiKey, config.ApiSecret, config.AccountSid);

        private Task<ResourceSet<ParticipantResource>> ParticipantsTask(string roomSid)
            => ParticipantResource.ReadAsync(
                new ReadParticipantOptions(roomSid) { Status = ParticipantStatus.Connected }, GetClient());

        private async Task<RoomModel> GetRooms(RoomResource room, Task<ResourceSet<ParticipantResource>> participantTask)
        {
            var participants = await participantTask;
            return roomMapper.Map(room, participants);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Twilio.Rest.Video.V1;
using Twilio.Rest.Video.V1.Room;
using VideoCall.Application.Models;

namespace VideoCall.Application
{
    public interface IRoomMapper
    {
        RoomModel Map(RoomResource room);
        RoomModel Map(RoomResource room, IEnumerable<ParticipantResource> participants);
    }
    public class RoomMapper : IRoomMapper
    {
        public RoomModel Map(RoomResource room)
        {
            return Map(room, Array.Empty<ParticipantResource>());
        }

        public RoomModel Map(RoomResource room, IEnumerable<ParticipantResource> participants)
        {
            return new RoomModel
            {
                Id = room.Sid,
                Name = room.UniqueName,
                MaxParticipants = room.MaxParticipants ?? 0,
                ParticipantCount = participants.ToArray().Length
            };
        }
    }
}

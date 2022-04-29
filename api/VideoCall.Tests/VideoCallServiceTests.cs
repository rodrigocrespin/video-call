using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using VideoCall.Application;
using Xunit;

namespace VideoCall.Tests
{
    public class VideoCallServiceTests
    {
        [Fact]
        public void Should_get_JWT()
        {
            var service = CreateService();

            var jwt = service.GetJwt();
            jwt.ShouldNotBeNull();
            jwt.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Should_get_all_rooms()
        {
            var service = CreateService();

            var rooms = await service.GetAllRooms();
            rooms.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task Should_create_room()
        {
            var service = CreateService();

            var name = $"test room {Guid.NewGuid()}";
            var maxParticipants = 5;

            var room = await service.CreateRoom(name, maxParticipants);
            room.Id.ShouldNotBeNull();
            room.Name.ShouldBe(name);
            room.MaxParticipants.ShouldBe(maxParticipants);
        }

        private IVideoCallService CreateService()
        {
            var config = TestsHelper.TwilioConfig;
            var options = new Mock<IOptionsSnapshot<TwilioConfig>>();
            options.Setup(x => x.Value).Returns(config);
            return new VideoCallService(options.Object, new RoomMapper(), Mock.Of<ILogger<VideoCallService>>());
        }
    }
}

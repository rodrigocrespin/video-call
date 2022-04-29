using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using VideoCall.Application;

namespace VideoCall.Tests
{
    public static class TestsHelper
    {
        private static TwilioConfig? twilioConfig;

        public static TwilioConfig TwilioConfig
        {
            get
            {
                Initialize();
                return twilioConfig!;
            }
        }

        private static void Initialize()
        {
            if (twilioConfig != null)
                return;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.tests.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<VideoCallServiceTests>();
            var configuration = builder.Build();

            twilioConfig = configuration.GetSection(TwilioConfig.ConfigKey).Get<TwilioConfig>();
        }
    }
}

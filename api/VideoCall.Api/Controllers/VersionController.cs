using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VideoCall.Api.Controllers
{
    public class VersionController : ControllerBase
    {
        private readonly ILogger<VersionController> logger;

        public VersionController(ILogger<VersionController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("api/version")]
        public ActionResult<VersionInfo> GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var buildTimestampStr = GetBuildTimestamp(assembly);
            var buildTimestamp = ConvertBuildTimestamp(buildTimestampStr);
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!;

            logger.LogDebug("Version {@CurrentVersion}", version);

            return new VersionInfo(version, buildTimestamp);
        }

        private static string? GetBuildTimestamp(Assembly assembly)
        {
            var buildInfoResourceName =
                assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains("build_info"));
            if (string.IsNullOrEmpty(buildInfoResourceName))
            {
                return null;
            }

            using var stream = assembly.GetManifestResourceStream(buildInfoResourceName);
            if (stream == null)
            {
                return null;
            }

            using var sr = new StreamReader(stream);
            return sr.ReadToEnd().Trim();
        }

        private static DateTime? ConvertBuildTimestamp(string? buildTimestampStr)
        {
            if (string.IsNullOrEmpty(buildTimestampStr) || !DateTime.TryParse(buildTimestampStr, out var buildTimestamp))
            {
                return null;
            }

            return buildTimestamp;
        }

        public class VersionInfo
        {
            public VersionInfo(string version, DateTime? buildTimestamp)
            {
                Version = version;
                BuildTimestamp = buildTimestamp;
            }

            public string Version { get; }
            public DateTime? BuildTimestamp { get; }
        }
    }
}

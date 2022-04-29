namespace VideoCall.Application
{
    public class TwilioConfig
    {
        public const string ConfigKey = "Twilio";

        /// <summary>
        /// The primary Twilio account SID, displayed prominently on your twilio.com/console dashboard.
        /// </summary>
        public string AccountSid { get; set; } = null!;

        /// <summary>
        /// Signing Key SID, also known as the API SID or API Key.
        /// </summary>
        public string ApiKey { get; set; } = null!;

        /// <summary>
        /// The API Secret that corresponds to the <see cref="ApiKey"/>.
        /// </summary>
        public string ApiSecret { get; set; } = null!;
    }
}

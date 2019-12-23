using Microsoft.Extensions.Configuration;

namespace IridiumLive.Data
{
    internal class AppSettingsService
    {
        private readonly IConfiguration _configuration;
        public AppSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetBaseUrl()
        {
            return _configuration.GetValue<string>("IridiumLiveSettings:BaseUrl");
        }
    }
}
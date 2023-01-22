using Microsoft.Extensions.Configuration;

namespace Cellarium.Utils;

public static class Constants
{
    private static IConfiguration? _configuration = null;
    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration is null)
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json");

                _configuration = builder.Build();
            }

            return _configuration;
        }
    }
}
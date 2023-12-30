using Microsoft.Extensions.Configuration;

namespace SpendManagement.IntegrationTests.Config
{
    public static class TestSettings
    {
        static TestSettings()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("testsettings.json", false, true)
               .Build();

            JwtOptions = config.GetSection("JwtOptions").Get<JwtOptions>();
        }

        public static JwtOptions? JwtOptions { get; }
    }
}

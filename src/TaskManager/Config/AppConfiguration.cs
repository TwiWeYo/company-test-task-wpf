using Microsoft.Extensions.Configuration;

namespace TaskManager.Config;

public static class AppConfiguration
{
    private static IConfigurationRoot _configuration;

    static AppConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();
    }

    public static T GetSection<T>(string sectionName) where T : new()
    {
        var section = new T();
        _configuration.GetSection(sectionName).Bind(section);
        return section;
    }

    public static string GetValue(string key)
    {
        return _configuration[key];
    }
}

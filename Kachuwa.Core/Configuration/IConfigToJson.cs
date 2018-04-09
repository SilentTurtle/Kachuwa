namespace Kachuwa.Configuration
{
    public interface IConfigToJson
    {
        bool SaveConnectionString(KachuwaConnectionStrings connectionString);
        bool SaveKachuwaConfig(KachuwaAppConfig config);
    }
}
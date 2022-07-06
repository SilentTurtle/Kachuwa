using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kachuwa.IdentityServerAdmin.Infrastructure
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Sex
    {
        Male,
        Female
    }
}
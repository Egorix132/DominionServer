using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dominion.SocketIoServer;

public static class OtherExtensions
{
    public static bool TryDeserializeObject<T>(this JToken token, out T? result)
    {
        try
        {
            result = JsonConvert.DeserializeObject<T>(token.ToString(), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
        catch (Exception e)
        {
            result = default;
            return false;
        }

        return true;
    }
}

using System;
using eDriven.Networking.Rpc.Json;
using JsonFx.Json;

namespace eDriven.Tests.Networking.JsonService
{
    /// <summary>
    /// Custom JSON serializer
    /// A wrapper to JsonFx
    /// Used to decouple Networking and JsonFx assemblies
    /// (this way Networking doesn't need to reference JsonFx)
    /// </summary>
    public class JsonFxSerializer : ISerializer
    {
        public string Serialize(object o)
        {
            return JsonWriter.Serialize(o);
        }

        public object Deserialize(string s, Type type)
        {
            return JsonReader.Deserialize(s, type);
        }
    }
}

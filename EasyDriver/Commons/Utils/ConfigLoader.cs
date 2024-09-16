using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Comfast.Commons.Utils;

/// <summary> Reads JSON file and sub objects</summary>
public class ConfigLoader {
    public static T Load<T>(string configFilePath, string? configKey) {
        var inputJson = File.ReadAllText(configFilePath);

        if (configKey != null) {
            var jObject = JObject.Parse(inputJson);
            var jsonValue = jObject[configKey]
                            ?? throw new Exception($"Not found key '{configKey}' in JSON: {configFilePath}");

            if (jsonValue is JValue v) return (T)v.Value;
            inputJson = jsonValue.ToString();
        }

        return JsonConvert.DeserializeObject<T>(inputJson)
               ?? throw new Exception($"Failed to deserialize JSON: '{inputJson.TrimToMaxLength(500)}'");
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Comfast.Commons.Utils;

public class ConfigLoader {
    public static T Load<T>(string configFilePath, string? configKey) {
        using var input = new StreamReader(configFilePath);
        var inputJson = input.ReadToEnd();

        if (configKey != null) {
            var jObject = JObject.Parse(inputJson);
            var jsonKey = jObject[configKey]
                          ?? throw new Exception($"Not found key {configKey} in JSON: {configFilePath}");
            inputJson = jsonKey.ToString();
        }

        return JsonConvert.DeserializeObject<T>(inputJson)
               ?? throw new Exception($"Failed to deserialize JSON: {inputJson.MaxLength(500)}");
    }
}
using System.Text.Json;

namespace BuildingBlocks.Core.Utils
{
    public static class JsonUtils
    {
        public static string Beautify(string json)
        {
            string formattedResponseText;
            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                formattedResponseText = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (JsonException)
            {
                return json;
            }

            return formattedResponseText;
        }
    }
}

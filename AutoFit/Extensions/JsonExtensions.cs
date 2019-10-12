using System.Text.Json;

namespace AutoFit
{
    public static class JsonExtensions
    {
        public static string TryGetPropertyOrDefault(this JsonElement source, string propertyName)
        {
            return source.TryGetProperty(propertyName, out var format)
                ? format.ToString()
                : null;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nut.AssemblyInfo
{
    public class Model
    {
        private static readonly string? version = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(a => a.Key == "ApplicationVersion").Select(a => a.Value).FirstOrDefault();

        public string Version => version ?? string.Empty;

        public List<ItemModel> BasicProperties { get; } = new();

        public List<ItemModel> MetadataProperties { get; } = new();

        public List<ItemModel> ProjectProperties { get; } = new();

        public bool WithoutAttribute { get; set; } = false;
    }

    public record ItemModel(string Key, string Value, string CSharpPropertyName);
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nut.AssemblyInfo
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        private readonly string[] basicAttributeNames = new[] {
            nameof(AssemblyCompanyAttribute),
            nameof(AssemblyCopyrightAttribute),
            nameof(AssemblyDescriptionAttribute),
            nameof(AssemblyTitleAttribute),
            nameof(AssemblyProductAttribute),
            nameof(AssemblyVersionAttribute),
            nameof(AssemblyTrademarkAttribute),
            nameof(AssemblyInformationalVersionAttribute),
            nameof(AssemblyFileVersionAttribute),
            nameof(AssemblyConfigurationAttribute),
            nameof(AssemblyCultureAttribute)
        };

        private readonly string[] projectProperties = new[]
        {
            "RootNamespace", "AssemblyName", "TargetFrameworkIdentifier", "TargetFrameworkMoniker", "TargetFrameworkVersion"
        };

        public void Execute(GeneratorExecutionContext context)
        {
            var model = new Model();
            model.BasicProperties.AddRange(GetBasicProperties(context));
            model.MetadataProperties.AddRange(GetMetadataProperties(context));
            model.ProjectProperties.AddRange(GetProjectProperties(context));

            var template = new ClassTemplate() { Model = model };
            var source = template.TransformText();
            context.AddSource($"Nut.AssemblyInfo.g", SourceText.From(source, Encoding.UTF8));
        }

        private Dictionary<string, string> GetBasicProperties(GeneratorExecutionContext context)
            => context.Compilation.Assembly.GetAttributes()
                .Where(attrData => !string.IsNullOrEmpty(attrData.AttributeClass?.Name) && basicAttributeNames.Contains(attrData.AttributeClass!.Name))
                .Select(attrData => ((RemovePreAndSuffix(attrData.AttributeClass!.Name), (string?)attrData.ConstructorArguments[0].Value ?? string.Empty)))
                .ToDictionary(x => x.Item1, x => x.Item2);

        private Dictionary<string, string> GetMetadataProperties(GeneratorExecutionContext context)
            => context.Compilation.Assembly.GetAttributes()
                .Where(attrData => attrData.AttributeClass?.Name == nameof(AssemblyMetadataAttribute))
                .Select(attrData => (CSharpUtil.ToValidIdentifier((string)attrData.ConstructorArguments[0].Value!), (string?)attrData.ConstructorArguments[1].Value ?? string.Empty))
                .Distinct(new KeyOnlyComparer())
                .ToDictionary(x => x.Item1, x => x.Item2);

        private Dictionary<string, string> GetProjectProperties(GeneratorExecutionContext context)
            => projectProperties.Select(pp =>
            {
                context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property." + pp, out var value);
                return (pp, value);
            }).Where(pv => !string.IsNullOrEmpty(pv.value))
            .ToDictionary(x => x.pp, x => x.value!);

        private string RemovePreAndSuffix(string attrName)
            => attrName.Substring(8, attrName.Length - 17);


        public void Initialize(GeneratorInitializationContext context)
        {
        }

        class KeyOnlyComparer : IEqualityComparer<(string, string)>
        {
            public bool Equals((string, string) x, (string, string) y)
                => x.Item1.Equals(y.Item1, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode((string, string) obj) => obj.Item1.GetHashCode();
        }
    }
}

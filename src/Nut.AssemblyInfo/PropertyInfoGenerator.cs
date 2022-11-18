using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Nut.AssemblyInfo
{
    [Generator]
    public class PropertyInfoGenerator : IIncrementalGenerator
    {
        private readonly string[] targetProperties = new[]
        {
            "RootNamespace", "AssemblyName", "TargetFrameworkIdentifier", "TargetFrameworkMoniker", "TargetFrameworkVersion"
        };

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var projectProperties = context.AnalyzerConfigOptionsProvider
                .SelectMany((p, ct) =>
                {
                    var gOptions = p.GlobalOptions;
                    return targetProperties.Select(p =>
                    {
                        var v = gOptions.TryGetValue("build_property." + p, out var value) ? value : null;
                        return new KeyValuePair<string, string>(p, v!);
                    }).Where(pair => pair.Value is not null);
                });

            context.RegisterSourceOutput(
                projectProperties.Collect(),
                GenerateSource!);
        }

        static void GenerateSource(SourceProductionContext spc, ImmutableArray<KeyValuePair<string, string>> attributes)
        {
            var model = new Model();
            model.WithoutAttribute = true;

            model.ProjectProperties.AddRange(attributes
                                    .Select(i => new ItemModel(i.Key, i.Value, i.Key)));
            var template = new ClassTemplate() { Model = model };
            var source = template.TransformText();

            spc.AddSource(
                "Nut.AssemblyInfo.ProjectProperty.g.cs",
                SourceText.From(source, Encoding.UTF8));
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Nut.AssemblyInfo
{
    [Generator]
    public class SourceGenerator : IIncrementalGenerator
    {
        private record Info(string Key, string Value, InfoType InfoType);

        private enum InfoType
        {
            AssemblyInfo,
            Metadata,
            Project
        }

        private static readonly string[] basicAttributeNames = new[] {
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

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var targets = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => s is AttributeSyntax,
                    transform: GetTarget)
                .Where(static m => m is not null);

            context.RegisterSourceOutput(
                targets.Collect(),
                GenerateSource!);
        }

        static Info? GetTarget(GeneratorSyntaxContext ctx, CancellationToken token)
        {
            var result = GetAssemblyInfoTarget(ctx, token);
            if (result is not null) return result;
            result = GetMetadataTarget(ctx, token);
            if (result is not null) return result;

            return result;
        }

        static Info? GetAssemblyInfoTarget(GeneratorSyntaxContext ctx, CancellationToken token)
        {
            var attributeNode = (AttributeSyntax)ctx.Node;

            if (ctx.SemanticModel.GetSymbolInfo(attributeNode, token).Symbol is not IMethodSymbol ctor)
                return null;

            var attributeType = ctor.ContainingType;
            if (attributeType == null)
                return null;

            if (!basicAttributeNames.Contains(attributeType.Name))
                return null;

            var key = attributeType.Name.Substring(8).Substring(0, attributeType.Name.Length - 8 - 9);
            var expression = attributeNode.ArgumentList!.Arguments[0].Expression;
            var value = ctx.SemanticModel.GetConstantValue(expression, token).ToString();
            return new Info(key, value, InfoType.AssemblyInfo);
        }

        static Info? GetMetadataTarget(GeneratorSyntaxContext ctx, CancellationToken token)
        {
            var attributeNode = (AttributeSyntax)ctx.Node;

            if (ctx.SemanticModel.GetSymbolInfo(attributeNode, token).Symbol is not IMethodSymbol ctor)
                return null;

            var attributeType = ctor.ContainingType;
            if (attributeType == null)
                return null;

            if (attributeType.Name != nameof(AssemblyMetadataAttribute))
                return null;

            var keyExpr = attributeNode.ArgumentList!.Arguments[0].Expression;
            var key = CSharpUtil.ToValidIdentifier(ctx.SemanticModel.GetConstantValue(keyExpr, token).ToString());
            var valueExpr = attributeNode.ArgumentList!.Arguments[1].Expression;
            var value = ctx.SemanticModel.GetConstantValue(valueExpr, token).ToString();
            return new Info(key, value, InfoType.Metadata);
        }


        static void GenerateSource(SourceProductionContext spc, ImmutableArray<Info> attributes)
        {
            var model = new Model();
            model.BasicProperties.AddRange(
                attributes
                    .Where(i => i.InfoType == InfoType.AssemblyInfo)
                    .Select(i => new KeyValuePair<string, string>(i.Key, i.Value)));
            model.MetadataProperties.AddRange(
                attributes
                    .Where(i => i.InfoType == InfoType.Metadata)
                    .Select(i => new KeyValuePair<string, string>(i.Key, i.Value)));
            var template = new ClassTemplate() { Model = model };
            var source = template.TransformText();

            spc.AddSource(
                "Nut.AssemblyInfo.g.cs",
                SourceText.From(source, Encoding.UTF8));
        }
    }
}

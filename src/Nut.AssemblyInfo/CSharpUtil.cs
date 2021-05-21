using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;

namespace Nut.AssemblyInfo
{
    public static class CSharpUtil
    {
        
        public static bool IsValidIdentifier(string value)
            => SyntaxFacts.IsValidIdentifier(value) &&
                SyntaxFacts.GetKeywordKind(value) == SyntaxKind.None &&
                SyntaxFacts.GetContextualKeywordKind(value) == SyntaxKind.None;

        public static string ToValidIdentifier(string text)
        {
            var replaced = text.Normalize();
            var notAllowChars = new Regex(@"[^\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc}|\p{Cf}]");
            replaced = notAllowChars.Replace(replaced, "_");
            var allowFirstChars = new Regex(@"(_|\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl})");
            if (!allowFirstChars.IsMatch(text[0].ToString()))
            {
                replaced = "_" + replaced;
            }

            if (SyntaxFacts.GetKeywordKind(replaced) != SyntaxKind.None || SyntaxFacts.GetContextualKeywordKind(replaced) != SyntaxKind.None)
            {
                replaced = "_" + replaced;
            }

            return replaced;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Nut.AssemblyInfo
{
    public partial class ClassTemplate
    {
        public Model? Model { get; set; }

        public string EscStr(string text) => text.Replace("\"", "\"\"");

        public string EscXmlStr(string text) => SecurityElement.Escape(text);
    }
}

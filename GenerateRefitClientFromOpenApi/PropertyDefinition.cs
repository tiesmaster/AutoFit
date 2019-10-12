using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateRefitClientFromOpenApi
{

    public class PropertyDefinition
    {
        public string TypeName { get; set; }
        public string IdentifierName { get; set; }
    }
}
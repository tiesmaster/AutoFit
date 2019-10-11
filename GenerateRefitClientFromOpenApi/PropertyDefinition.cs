using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateRefitClientFromOpenApi
{

    public class PropertyDefinition
    {
        public string Type { get; set; }
        public string Identifier { get; set; }
    }
}
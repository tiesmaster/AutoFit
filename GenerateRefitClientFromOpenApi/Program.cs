using System;

using Microsoft.CodeAnalysis.CSharp;

namespace GenerateRefitClientFromOpenApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var hoiNode = SyntaxFactory.ParseExpression("Console.WriteLine(\"Hello World!\");");
            Console.WriteLine(hoiNode);
        }
    }
}
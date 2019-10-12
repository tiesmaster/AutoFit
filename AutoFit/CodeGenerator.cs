using System.IO;
using System.Text.Json;

namespace AutoFit
{
    public class CodeGenerator
    {
        private readonly string _inputFile;
        private readonly SourceEmitter _sourceEmitter;

        public CodeGenerator(string inputFile, string outputPath, string namespaceName)
        {
            _inputFile = inputFile;
            _sourceEmitter = new SourceEmitter(namespaceName, outputPath);
        }

        public void Emit()
        {
            using var document = JsonDocument.Parse(File.OpenRead(_inputFile));

            var dtoDefinitions = document.RootElement.GetProperty("definitions");
            foreach (var dtoDefinition in dtoDefinitions.EnumerateObject())
            {
                GenerateDto(dtoDefinition);
            }
        }

        private void GenerateDto(JsonProperty jsonDtoDefinition)
        {
            var dtoDefinition = OpenApiParser.ParseDefinition(jsonDtoDefinition);
            _sourceEmitter.EmitDto(dtoDefinition);
        }
    }
}
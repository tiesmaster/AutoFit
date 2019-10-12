namespace AutoFit
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var codeGenerator = new CodeGenerator(
                inputFile: "rdc-openapi.json",
                outputPath: @"C:\src\buddy\rabodirectconnect-api\RaboDirectConnectApi.Dto",
                namespaceName: "Buddy.RaboDirectConnectApi.Dto");

            codeGenerator.Emit();
        }
    }
}
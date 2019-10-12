namespace AutoFit
{
    public class TypeDefinition
    {
        public string Name { get; set; }
        public string Namespace { get; set; }

        public static implicit operator TypeDefinition(string name)
        {
            return new TypeDefinition { Name = name };
        }
    }
}
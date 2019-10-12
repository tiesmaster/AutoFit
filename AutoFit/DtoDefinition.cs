using System.Collections.Generic;

namespace AutoFit
{
    public class DtoDefinition
    {
        public string Name { get; set; }
        public IEnumerable<PropertyDefinition> Properties { get; set; }
    }
}

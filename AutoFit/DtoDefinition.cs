using System.Collections.Generic;
using System.Linq;

namespace AutoFit
{
    public class DtoDefinition
    {
        public string Name { get; set; }
        public IEnumerable<PropertyDefinition> Properties { get; set; }

        public IEnumerable<string> RequestedNamespaces
            => Properties
                .Where(x => x.TypeDefinition.Namespace != null)
                .Select(x => x.TypeDefinition.Namespace)
                .Distinct();
    }
}

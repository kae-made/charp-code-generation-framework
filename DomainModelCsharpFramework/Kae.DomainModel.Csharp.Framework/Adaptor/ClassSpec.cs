using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class ClassSpec
    {
        public string Name { get; set; }
        public string KeyLetter { get; set; }
        public IDictionary<string, PropSpec> Properties { get; set; }
        public IDictionary<string, OperationSpec> Operations { get; set; }
        public IDictionary<string, LinkSpec> Links { get; set; }
        public IDictionary<string, OperationSpec> Events { get; set; }
    }
}

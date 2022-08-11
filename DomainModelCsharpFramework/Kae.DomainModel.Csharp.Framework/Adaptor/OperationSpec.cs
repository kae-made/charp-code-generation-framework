using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class OperationSpec
    {
        public string Name { get; set; }
        public IDictionary<string, ParamSpec> Parameters { get; set; }
        public ParamSpec.DataType ReturnType { get; set; }
    }
}

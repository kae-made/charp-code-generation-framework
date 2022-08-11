using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kae.DomainModel.Csharp.Framework.Adaptor
{
    public class ParamSpec
    {
        public enum DataType
        {
            String,
            Integer,
            Real,
            Boolean,
            DateTime,
            Void,
            Enum,
            Complex
        }
        public string Name { get; set; }
        public DataType TypeKind { get; set; }
        public bool IsArray { get; set; }
    }
}

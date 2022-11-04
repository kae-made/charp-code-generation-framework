using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kae.Utility.Json
{
    public static class JsonHelper
    {
        public static object GetJsonContent(JToken token, IDictionary<string, object> dataHolder)
        {
            object result = null;
            switch (token.Type)
            {
                case JTokenType.Object:
                    var jobject = (JObject)token;
                    var ownDataHolder = new Dictionary<string, object>();
                    foreach (var childProp in jobject.Children())
                    {
                        if (childProp.Type == JTokenType.Property)
                        {
                            var property = (JProperty)childProp;
                            ownDataHolder.Add(property.Name, GetJsonContent(property.Value, ownDataHolder));
                        }
                    }
                    result = ownDataHolder;
                    break;
                case JTokenType.Property:
                    throw new IndexOutOfRangeException("Property shouldn't exist!");
                    break;
                case JTokenType.Boolean:
                case JTokenType.Array:
                case JTokenType.Bytes:
                case JTokenType.Date:
                case JTokenType.Float:
                case JTokenType.Guid:
                case JTokenType.Integer:
                case JTokenType.String:
                case JTokenType.TimeSpan:
                case JTokenType.Uri:
                case JTokenType.None:
                case JTokenType.Null:
                case JTokenType.Raw:
                    result = ((JValue)token).Value;
                    break;
                case JTokenType.Undefined:
                    break;

            }
            return result;
        }
    }
}

#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: LoadJsonHelpers.cs
// Date Created: 2015/12/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Tests.Helpers
{
    public static class LoadJsonHelpers
    {

        public static T DeserializeData<T>(string searchString) where T : class
        {
            var jsonText = TestFileHelpers.GetTestFileContent(searchString);
            var contractResolver = new PrivateSetterJsonDefaultContractResolver();
            var settings = new JsonSerializerSettings { ContractResolver = contractResolver };

            return JsonConvert.DeserializeObject<T>(jsonText, settings);
        }


        public static T DeserializeDataWithSingleAlteration<T>(string searchString, object value, params object [] accessKeys) where T : class
        {
            var jsonText = TestFileHelpers.GetTestFileContent(searchString);
            var jObject = JObject.Parse(jsonText);

            switch (accessKeys.Length)
            {
                case 1:
                    jObject[accessKeys[0]] = new JValue(value);
                    break;
                case 2:
                    jObject[accessKeys[0]][accessKeys[1]] = new JValue(value);
                    break;
                case 3:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]] = new JValue(value);
                    break;
                case 4:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]] = new JValue(value);
                    break;
                case 5:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]] = new JValue(value);
                    break;
                case 6:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]][accessKeys[5]] = new JValue(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var contractResolver = new PrivateSetterJsonDefaultContractResolver();
            var settings = new JsonSerializerSettings { ContractResolver = contractResolver };

            return JsonConvert.DeserializeObject<T>(jObject.ToString(), settings);
        }


        public class PrivateSetterJsonDefaultContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }
    }
}
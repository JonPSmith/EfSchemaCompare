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
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters = new[] { new TypeConverter() }
            };

            return JsonConvert.DeserializeObject<T>(jsonText, settings);
        }

        public static T DeserializeObjectWithSingleAlteration<T>(string searchString, object value, params object [] accessKeys) where T : class
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
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters = new[] { new TypeConverter() }
            };

            return JsonConvert.DeserializeObject<T>(jObject.ToString(), settings);
        }

        public static T DeserializeArrayWithSingleAlteration<T>(string searchString, object value, params object[] accessKeys) where T : class
        {
            var jsonText = TestFileHelpers.GetTestFileContent(searchString);
            var jArray = JArray.Parse(jsonText);

            switch (accessKeys.Length)
            {
                case 1:
                    jArray[accessKeys[0]] = new JValue(value);
                    break;
                case 2:
                    jArray[accessKeys[0]][accessKeys[1]] = new JValue(value);
                    break;
                case 3:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]] = new JValue(value);
                    break;
                case 4:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]] = new JValue(value);
                    break;
                case 5:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]] = new JValue(value);
                    break;
                case 6:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]][accessKeys[5]] = new JValue(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var contractResolver = new PrivateSetterJsonDefaultContractResolver();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters = new[] { new TypeConverter() }
            };

            return JsonConvert.DeserializeObject<T>(jArray.ToString(), settings);
        }

        public static T DeserializeObjectWithSingleRemoval<T>(string searchString, params object[] accessKeys) where T : class
        {
            var jsonText = TestFileHelpers.GetTestFileContent(searchString);
            var jObject = JObject.Parse(jsonText);

            switch (accessKeys.Length)
            {
                case 1:
                    jObject[accessKeys[0]].Remove();
                    break;
                case 2:
                    jObject[accessKeys[0]][accessKeys[1]].Remove();
                    break;
                case 3:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]].Remove();
                    break;
                case 4:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]].Remove();
                    break;
                case 5:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]].Remove();
                    break;
                case 6:
                    jObject[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]][accessKeys[5]].Remove();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var contractResolver = new PrivateSetterJsonDefaultContractResolver();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters = new[] { new TypeConverter() }
            };

            return JsonConvert.DeserializeObject<T>(jObject.ToString(), settings);
        }

        public static T DeserializeArrayWithSingleRemoval<T>(string searchString, params object[] accessKeys) where T : class
        {
            var jsonText = TestFileHelpers.GetTestFileContent(searchString);
            var jArray = JArray.Parse(jsonText);

            switch (accessKeys.Length)
            {
                case 1:
                    jArray[accessKeys[0]].Remove();
                    break;
                case 2:
                    jArray[accessKeys[0]][accessKeys[1]].Remove();
                    break;
                case 3:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]].Remove();
                    break;
                case 4:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]].Remove();
                    break;
                case 5:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]].Remove();
                    break;
                case 6:
                    jArray[accessKeys[0]][accessKeys[1]][accessKeys[2]][accessKeys[3]][accessKeys[4]][accessKeys[5]].Remove();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var contractResolver = new PrivateSetterJsonDefaultContractResolver();
            var settings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Converters = new[] { new TypeConverter() }
            };

            return JsonConvert.DeserializeObject<T>(jArray.ToString(), settings);
        }


        public class TypeConverter : JsonConverter
        {
            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Type));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var typeName = (string) reader.Value;
                var typeToReturn = Type.GetType(typeName);
                if (typeToReturn == null)
                    throw new InvalidOperationException(
                        string.Format("Could not convert the type string {0} into a type.", typeName));
                return typeToReturn;
            }
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
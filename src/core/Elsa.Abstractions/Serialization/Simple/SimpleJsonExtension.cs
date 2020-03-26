using SimpleJson;
using Elsa.Serialization;
using Elsa.Serialization.Simple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Elsa.Serialization.Simple
{
    public static class SimpleJsonExtension
    {
        private const string INDENT_STRING = "  ";

        public static JsonObject Merge(JsonObject srcObject, JsonObject mergeObject)
        {
            return Merge(srcObject, mergeObject, new List<KeyValuePair<string, string>>() { });
        }


        public static JsonObject Merge(JsonObject srcObject, JsonObject mergeObject, List<KeyValuePair<string, string>> keyProperties)
        {
            JsonObject result = srcObject;
            foreach (string prop in mergeObject.GetDynamicMemberNames())
            {
                if (result.Keys.Contains(prop))
                {
                    if (mergeObject[prop] is JsonArray)
                    {
                        if ((mergeObject[prop] as JsonArray).Count > 0)
                        {
                            srcObject[prop] = Merge((srcObject[prop] as JsonArray), (mergeObject[prop] as JsonArray), prop, keyProperties);
                        }
                    }
                    else if (mergeObject[prop] is JsonObject)
                    {
                        if (result[prop] is JsonObject)
                        {
                            result[prop] = Merge(result[prop] as JsonObject, mergeObject[prop] as JsonObject, keyProperties);
                        }
                        else
                        {
                            result[prop] = mergeObject[prop];
                        }
                    }
                    else
                    {
                        result[prop] = mergeObject[prop];
                    }
                }
                else
                {
                    result.Add(prop, mergeObject[prop]);
                }
            }

            return result;
        }

        public static JsonArray Merge(JsonArray srcArray, JsonArray mergeArray, string keyProperty, List<KeyValuePair<string, string>> keyProperties)
        {
            JsonArray result = srcArray;
            if (srcArray == null || srcArray.Count <= 0)
            {
                return mergeArray;
            }
            else
            {
                if (mergeArray.Count > 0)
                {
                    var prop = keyProperties.FirstOrDefault(item => string.Equals(item.Key, keyProperty)).Value;

                    foreach (var item in mergeArray)
                    {
                        if (item is JsonObject)
                        {
                            if (!string.IsNullOrWhiteSpace(prop))
                            {
                                string propValue = ((item as JsonObject)[prop]).ToString();
                                if (!string.IsNullOrWhiteSpace(propValue))
                                {
                                    var resultItem = result.Find(x => string.Equals((x as JsonObject)[prop], propValue));
                                    if (resultItem != null)
                                    {
                                        resultItem = Merge((resultItem as JsonObject), (item as JsonObject), keyProperties);
                                        continue;
                                    }
                                }
                            }

                            result.Add(item);
                        }
                        else if (!result.Contains(item))
                        {
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        public static dynamic ConvertToJson(DataTable dtObject)
        {
            var result = new SimpleJson.JsonArray();

            foreach (DataRow row in dtObject.Rows)
            {
                dynamic record = new SimpleJson.JsonObject();
                foreach (DataColumn col in dtObject.Columns)
                {
                    record[col.ColumnName] = row[col.ColumnName];

                    // if date returned from database is kind unknown then we assume its UTC
                    if (col.DataType.FullName == "System.DateTime")
                    {
                        DateTime dt = (DateTime)row[col.ColumnName];
                        if (dt.Kind == DateTimeKind.Unspecified) record[col.ColumnName] = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    }
                }
                result.Add(record);
            }
            return result;

        }
        public static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;

                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;

                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;

                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;

                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;

                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

        public static bool IsJsonString(string stringContent)
        {
            bool isJsonString = false;
            bool firstNonWhitespace = false;
            int strLength = stringContent.Length;
            for (int i = 0; i < strLength && !firstNonWhitespace; i++)
            {
                char currentChar = stringContent[i];
                firstNonWhitespace = !Char.IsWhiteSpace(currentChar);
                isJsonString = firstNonWhitespace && currentChar == '{';
            }
            return isJsonString;
        }


    }
}


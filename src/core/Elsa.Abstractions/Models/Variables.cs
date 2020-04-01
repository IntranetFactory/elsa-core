using Elsa.Converters;
using Elsa.Expressions;
using Elsa.ExpressionTypes;
using Elsa.Serialization.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;

namespace Elsa.Models
{
    public class Variables : Dictionary<string, Variable>
    {
        public Variables() : base(0, StringComparer.OrdinalIgnoreCase)
        {
        }

        public Variables(Variables other) : this((IEnumerable<KeyValuePair<string, Variable>>)other)
        {
        }

        public Variables(IEnumerable<KeyValuePair<string, Variable>> dictionary)
        {
            foreach (var item in dictionary)
                this[item.Key] = item.Value;
        }

        public Variables(IEnumerable<KeyValuePair<string, object>> dictionary)
        {
            foreach (var item in dictionary)
                SetVariable(item.Key, item.Value);
        }

        public object GetVariable(string name)
        {
            return ContainsKey(name) ? this[name]?.Value : default;
        }

        public T GetVariable<T>(string name)
        {
            if (!HasVariable(name))
                return default;

            var value = this[name]?.Value;

            if (value == null)
                return default;

            if (value is T v)
                return v;

            // this is used to return a collection of items for UserTask activity
            if (value.GetType().Name == "JArray")
            {
                JArray jArray = JArray.FromObject(value);
                var items = jArray.ToObject<T>();
                return (T)items;
            }

            if (typeof(T).Name.StartsWith("IWorkflowExpression"))
            {
                var json = JsonConvert.SerializeObject(value);
                dynamic expression = SimpleJson.SimpleJson.DeserializeObject(json);

                if (typeof(T).IsGenericType)
                {
                    Type typeArgument = typeof(T).GetGenericArguments()[0];
                    Type genericClass = null;

                    switch (expression["Type"])
                    {
                        case "Literal":
                            genericClass = typeof(LiteralExpression<>);
                            break;

                        case "JavaScript":
                            genericClass = typeof(JavaScriptExpression<>);
                            break;
                        case "Liquid":
                            genericClass = typeof(LiquidExpression<>);
                            break;
                    }

                    Type constructedClass = genericClass.MakeGenericType(typeArgument);
                    object createdObject = Activator.CreateInstance(constructedClass, expression["Expression"]);
                    return (T)createdObject;
                }
                else
                {
                    switch (expression["Type"])
                    {
                        case "Literal":
                            IWorkflowExpression literalExpression = new LiteralExpression(expression["Expression"]);
                            return (T)literalExpression;

                        case "JavaScript":
                            IWorkflowExpression javaScriptExpression = new JavaScriptExpression(expression["Expression"]);
                            return (T)javaScriptExpression;

                        case "Liquid":
                            IWorkflowExpression liquidExpression = new LiquidExpression(expression["Expression"]);
                            return (T)liquidExpression;
                    }
                }
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(value);
        }

        public Variables SetVariable(string name, object value)
        {
            this[name] = new Variable(value);
            return this;
        }

        public Variables SetVariable(string name, Variable variable)
        {
            this[name] = variable;
            return this;
        }

        public void SetVariables(Variables variables) =>
            SetVariables((IEnumerable<KeyValuePair<string, Variable>>)variables);

        public Variables SetVariables(IEnumerable<KeyValuePair<string, Variable>> variables)
        {
            foreach (var variable in variables)
                SetVariable(variable.Key, variable.Value);

            return this;
        }

        public bool HasVariable(string name) => ContainsKey(name);

    }
}
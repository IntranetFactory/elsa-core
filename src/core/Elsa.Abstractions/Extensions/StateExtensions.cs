using System;
using System.Runtime.CompilerServices;
using Elsa.Models;
using Newtonsoft.Json.Linq;

namespace Elsa.Extensions
{
    public static class StateExtensions
    {
        public static T GetState<T>(this Variables state, [CallerMemberName]string? key = null, Func<T>? defaultValue = null)
        {
            if (!state.HasVariable(key))
            {
                var value = defaultValue != null ? defaultValue() : default;
                state.SetVariable(key, value);
            }
            
            if(state.HasVariable(key))
            {
                if(state[key] != null && state[key].Value != null)
                {
                    if(state[key].Value.GetType().Name == "JArray")
                    {
                        var json = state[key].Value.ToString();
                        var itemsArray = JArray.Parse(json);

                        if (itemsArray.First != null)
                        {
                            if((string)itemsArray.First == "")
                            {
                                var value = defaultValue != null ? defaultValue() : default;
                                state.SetVariable(key, value);
                            }
                        }
                    }
                }
            }

            return state.GetVariable<T>(key);
        }
        
        public static void SetState(this Variables state, object value, [CallerMemberName]string? key = null) => state.SetVariable(key, value);
    }
}
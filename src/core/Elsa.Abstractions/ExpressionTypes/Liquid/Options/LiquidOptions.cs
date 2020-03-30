using System;
using System.Collections.Generic;

namespace Elsa.ExpressionTypes.Liquid.Options
{
    public class LiquidOptions
    {
        public Dictionary<string, Type> FilterRegistrations { get; }  = new Dictionary<string, Type>();
    }
}

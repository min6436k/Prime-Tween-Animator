using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ODY.PrimeTweenAnimation
{
    /// <summary>
    /// Attribute that links IPrimeAnimStrategy and TweenType.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class PrimeAnimStrategyAttribute : Attribute
    {
        private static Dictionary<TweenType, Type> _typeCache;

        internal readonly TweenType TweenType;

        internal PrimeAnimStrategyAttribute(TweenType tweenType) => TweenType = tweenType;

        internal static bool GetClassType(TweenType tweenType, out Type targetType)
        {
            _typeCache ??= typeof(PrimeAnimContainer)
                .GetNestedTypes(BindingFlags.Public)
                .Select(t => new { Type = t, Attribute = t.GetCustomAttribute<PrimeAnimStrategyAttribute>() })
                .Where(x => x.Attribute != null)
                .ToDictionary(x => x.Attribute.TweenType, x => x.Type);

            return _typeCache.TryGetValue(tweenType, out targetType);
        }
    }
}
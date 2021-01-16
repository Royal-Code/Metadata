using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RoyalCode.Metadata.Models;

namespace RoyalCode.Metadata.Factories
{
    internal static class SettingsActivator
    {
        private static readonly ConcurrentDictionary<Type, object> activators = new ();
        
        internal static TSettings Activate<TSettings>(MetaModel model, MetaBase origin)
        {
            var activator = (Func<MetaModel, MetaBase, TSettings>) activators
                .GetOrAdd(typeof(TSettings), CreateActivator<TSettings>);

            return activator(model, origin);
        }

        private static Func<MetaModel, MetaBase, TSettings> CreateActivator<TSettings>(Type type)
        {
            var ctors = type.GetConstructors()
                .Where(FilterValidCtor)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToList();

            if (ctors.Any())
            {
                var resultParam = Expression.Parameter(type, "result");
                
                foreach (var ctor in ctors)
                {
                    switch (ctor.GetParameters().Length)
                    {
                        case 2:
                            break;
                        case 1:
                            break;
                        case 0:
                            break;
                    }
                }
            }
            else
            {
                // throw
            }
        }

        private static bool FilterValidCtor(ConstructorInfo info)
            => NoParameters(info) || HasOrigin(info) || HasModelAndOrigin(info);
        
        private static bool NoParameters(ConstructorInfo info) => info.GetParameters().Length is 0;
        
        private static bool HasOrigin(ConstructorInfo info)
        {
            var parms = info.GetParameters();
            return parms.Length is 1 && parms[0].ParameterType == typeof(MetaBase);
        }

        private static bool HasModelAndOrigin(ConstructorInfo info)
        {
            var parms = info.GetParameters();
            return parms.Length is 2
                   && parms[0].ParameterType == typeof(MetaModel)
                   && parms[1].ParameterType == typeof(MetaBase);
        }
    }
}
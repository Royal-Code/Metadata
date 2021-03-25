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
        private static readonly MethodInfo getParentSettingsMethod = typeof(SettingsActivator).GetMethod(nameof(GetParentSettings));

        private static readonly ConcurrentDictionary<Type, object> activators = new();

        internal static TSettings Activate<TSettings>(MetaBase origin)
        {
            var activator = (Func<MetaBase, TSettings>)activators
                .GetOrAdd(typeof(TSettings), CreateActivator<TSettings>);

            return activator(origin);
        }

        private static Func<MetaBase, TSettings> CreateActivator<TSettings>(Type type)
        {
            var ctors = type.GetConstructors()
                .Where(FilterValidCtor)
                .OrderByDescending(c => c.GetParameters().Length)
                .ToList();

            if (ctors.Any())
            {
                var resultParam = Expression.Parameter(type, "result");
                var originParam = Expression.Parameter(typeof(MetaBase), "origin");

                Expression resultAssign;

                var ctor = ctors.First();

                switch (ctor.GetParameters().Length)
                {
                    case 2:

                        var settingsCtorParameter = ctor.GetParameters()[1];

                        resultAssign = Expression.Assign(resultParam,
                            Expression.New(ctor, 
                                originParam,
                                Expression.Call(
                                    getParentSettingsMethod.MakeGenericMethod(settingsCtorParameter.ParameterType),
                                    originParam,
                                    Expression.Constant(settingsCtorParameter))));

                        break;
                    case 1:
                        
                        resultAssign = Expression.Assign(resultParam,
                            Expression.New(ctor, originParam));

                        break;
                    case 0:
                        
                        resultAssign = Expression.Assign(resultParam,
                            Expression.New(ctor));
                        
                        break;

                    default:
                        throw new Exception();
                }

                var body = Expression.Block(type, new ParameterExpression[] { resultParam }, resultAssign, resultParam);

                var lambda = Expression.Lambda<Func<MetaBase, TSettings>>(body, originParam);
                return lambda.Compile();
            }
            else
            {
                var message = $"Impossible to activate the settings of type {type.FullName}.\n"
                    + "None valid constructor found.";
                throw new NotSupportedException(message);
            }

            throw new NotImplementedException();
        }

        private static bool FilterValidCtor(ConstructorInfo info)
            => NoParameters(info) || HasMetaBase(info) || HasMetaBaseAndParrentSettings(info);

        private static bool NoParameters(ConstructorInfo info) => info.GetParameters().Length is 0;

        private static bool HasMetaBase(ConstructorInfo info)
        {
            var parms = info.GetParameters();
            return parms.Length is 1 && parms[0].ParameterType == typeof(MetaBase);
        }

        private static bool HasMetaBaseAndParrentSettings(ConstructorInfo info)
        {
            var parms = info.GetParameters();
            return parms.Length is 2
                   && parms[0].ParameterType == typeof(MetaBase)
                   && parms[1].ParameterType.IsClass;
        }

        private static TSettings GetParentSettings<TSettings>(MetaBase metaBase, ParameterInfo parameter)
            where TSettings : class
        {
            try
            {
                return metaBase.Parent.GetOrCreateSettings<TSettings>();
            }
            catch (NotSupportedException ex)
            {
                var message = $"Impossible to activate the settings of type {parameter.Member.DeclaringType.FullName}.\n"
                    + $"The constructor parameter '{parameter.Name}' has an invalid type ({parameter.ParameterType.FullName}) for an settings.\nCause:\n"
                    + $"  {ex.Message.Replace("\n", "\n  ")}";

                throw new NotSupportedException(message, ex);
            }
        }
    }
}
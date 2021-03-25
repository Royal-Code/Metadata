using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RoyalCode.Metadata.Factories;

namespace RoyalCode.Metadata.Models
{
    /// <summary>
    /// Singleton que representa os meta-modelos criados e relacionados aos tipos/classes.
    /// </summary>
    public sealed class MetaModel : MetaBase
    {
        /// <summary>
        /// Instância deste singleton.
        /// </summary>
        /// <returns></returns>
        public static readonly MetaModel Instance = new();

        private readonly List<IMetaModelSettingsFactory> factories = new();
        private readonly List<MetaType> metaTypes = new();
        private readonly object _creationLock = new();

        public override MetaModel Model => this;

        public override MetaBase Parent => this;

        internal TSettings TryActivateSettings<TSettings>(MetaBase metaBase)
            where TSettings : class
        {
            return factories
                .OfType<IMetaModelSettingsFactory<TSettings>>()
                .FirstOrDefault()
                ?.Create(metaBase)
                ?? SettingsActivator.Activate<TSettings>(metaBase);
        }

        public void AddSettingsFactory<TSettings>(IMetaModelSettingsFactory<TSettings> factory) => factories.Add(factory);

        public MetaType GetMetaType(TypeInfo typeInfo)
        {
            return metaTypes.FirstOrDefault(m => m.TypeInfo == typeInfo) ?? CreateMetaType(typeInfo);
        }

        public TSettings GetOrCreateMetaTypeSettings<TSettings>(TypeInfo typeInfo)
            where TSettings : class
        {
            var metaType = GetMetaType(typeInfo);
            return metaType.GetOrCreateSettings<TSettings>();
        }

        private MetaType CreateMetaType(TypeInfo typeInfo)
        {
            lock (_creationLock)
            {
                var meta = metaTypes.FirstOrDefault(m => m.TypeInfo == typeInfo);
                if (meta is null)
                {
                    meta = new MetaType(this, this, typeInfo);
                    metaTypes.Add(meta);
                }
                return meta;
            }
        }
    }

    public static class MetaModelExtensions
    {
        public static MetaType GetMetaType(this MetaModel metaModel, Type type)
            => metaModel.GetMetaType(type.GetTypeInfo());

        public static MetaType GetMetaType<TType>(this MetaModel metaModel)
            => metaModel.GetMetaType(typeof(TType).GetTypeInfo());

        public static TSettings GetOrCreateMetaTypeSettings<TSettings>(this MetaModel metaModel, Type type)
            where TSettings : class
            => metaModel.GetOrCreateMetaTypeSettings<TSettings>(type.GetTypeInfo());

        public static TSettings GetOrCreateMetaTypeSettings<TType, TSettings>(this MetaModel metaModel)
            where TSettings : class
            => metaModel.GetOrCreateMetaTypeSettings<TSettings>(typeof(TType).GetTypeInfo());
    }
}

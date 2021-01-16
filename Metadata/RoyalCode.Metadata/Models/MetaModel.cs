using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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

        public override MetaModel Model => this;

        public override MetaBase Parent => this;

        internal TSettings TryCreateSettings<TSettings>(MetaBase metaBase)
            where TSettings: class
        {
            return factories
                .OfType<IMetaModelSettingsFactory<TSettings>>()
                .FirstOrDefault()
                ?.Create(this, metaBase);
        }

    }
}

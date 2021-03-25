using RoyalCode.Metadata.Models;
using System;

namespace RoyalCode.Metadata.Settings
{
    /// <summary>
    /// <para>
    ///     Tipo base para criação de settings.
    /// </para>
    /// <para>
    ///     Contém o metadata base deste settings e o metamodel.
    /// </para>
    /// <list type="bullet">
    ///     <listheader>
    ///         Veja mais opções de settings base utilizando tipos genéricos como:
    ///     </listheader>
    ///     <item><see cref="SettingsBase{TSettingsParent}"/>.</item>
    /// </list>
    /// </summary>
    public abstract class SettingsBase
    {
        public MetaBase Metadata { get; protected set; }

        public MetaModel MetaModel { get; protected set; }

        protected SettingsBase(MetaBase metadata)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            MetaModel = metadata.Model;
        }
    }

    /// <summary>
    /// <para>
    ///     Tipo base para criação de settings com referência ao settings pai deste, 
    ///     conhecido pelo tipo genérico <typeparamref name="TSettingsParent"/>.
    /// </para>
    /// <para>
    ///     O settings pai será obtido do pai do metadata atual.
    /// </para>
    /// </summary>
    /// <typeparam name="TSettingsParent"></typeparam>
    public class SettingsBase<TSettingsParent> : SettingsBase
        where TSettingsParent : class
    {
        protected SettingsBase(MetaBase metadata, TSettingsParent parent) 
            : base(metadata)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public TSettingsParent Parent { get; protected set; }
    }
}

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
    ///     <item><see cref="SettingsBase{TSettingsModel}"/>.</item>
    ///     <item><see cref="SettingsBase{TSettingsModel, TSettingsParent}"/>.</item>
    /// </list>
    /// </summary>
    public class SettingsBase
    {
        public MetaBase Metadata { get; protected set; }

        public MetaModel MetaModel { get; protected set; }

        protected SettingsBase() { }

        protected SettingsBase(MetaBase metadata, MetaModel metaModel)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            MetaModel = metaModel ?? throw new ArgumentNullException(nameof(metaModel));
        }
    }

    /// <summary>
    /// <para>
    ///     Tipo base para criação de settings cujo settings vinculado ao model é conhecido
    ///     pelo tipo <typeparamref name="TSettingsModel"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TSettingsModel">Tipo do settings vinculado a um <see cref="MetaModel"/>.</typeparam>
    public class SettingsBase<TSettingsModel> : SettingsBase
        where TSettingsModel : class
    {
        protected SettingsBase() { }

        protected SettingsBase(MetaBase metadata, MetaModel metaModel, TSettingsModel model) 
            : base(metadata, metaModel)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public TSettingsModel Model { get; protected set; }
    }

    /// <summary>
    /// <para>
    ///     Tipo base para criação de settings cujo settings vinculado ao model é conhecido
    ///     pelo tipo <typeparamref name="TSettingsModel"/>, e o settings pai deste também
    ///     é conhecido pelo tipo genérico <typeparamref name="TSettingsParent"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    /// <typeparam name="TSettingsParent"></typeparam>
    public class SettingsBase<TSettingsModel, TSettingsParent> : SettingsBase<TSettingsModel>
        where TSettingsModel : class
        where TSettingsParent : class
    {
        protected SettingsBase() { }

        protected SettingsBase(MetaBase metadata, MetaModel metaModel, TSettingsModel model, TSettingsParent parent) 
            : base(metadata, metaModel, model)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public TSettingsParent Parent { get; protected set; }
    }
}

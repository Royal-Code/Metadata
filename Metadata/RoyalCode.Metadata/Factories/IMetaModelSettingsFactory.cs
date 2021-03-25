using RoyalCode.Metadata.Models;

namespace RoyalCode.Metadata.Factories
{
    /// <summary>
    /// Componente utilizado para criação personalizada de settings para os meta-modelos.
    /// </summary>
    /// <typeparam name="TSettings">Tipo do settings que será criado.</typeparam>
    public interface IMetaModelSettingsFactory<out TSettings> : IMetaModelSettingsFactory {
        
        /// <summary>
        /// Realiza a criação de um settings
        /// </summary>
        /// <param name="metaBase"></param>
        /// <returns></returns>
        TSettings Create(MetaBase metaBase);
    }

    /// <summary>
    /// <para>
    ///     Componente utilizado para criação personalizada de settings para os meta-modelos.
    /// </para>
    /// <para>
    ///     Para criar uma fábrica e registrar no <see cref="MetaModel"/>
    ///     implemente a interface com parâmetro genérico do tipo de settings,
    ///     <see <see cref="IMetaModelSettingsFactory{TSettings}"/>
    /// </para>
    /// </summary>
    public interface IMetaModelSettingsFactory { }
}
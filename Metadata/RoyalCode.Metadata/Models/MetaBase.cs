using System;
using System.Collections.Generic;

namespace RoyalCode.Metadata.Models
{
    /// <summary>
    /// <para>
    ///     Classe base para metadados.
    /// </para>
    /// <para>
    ///     Cada tipo de modelo de metadados poderá ter várias configurações relacionadas a si, feitas através dos
    ///     Settings.
    /// </para>
    /// <para>
    ///     Os modelos podem armazenar Settings por tipo de classe. Veja mais nos métodos: 
    ///     <see cref="AddSettings{TSettings}(TSettings)"/>,
    ///     <see cref="HasSettings{TSettings}"/>,
    ///     <see cref="GetSettings{TSettings}"/>.
    /// </para>
    /// </summary>
    public abstract class MetaBase
    {
        /// <summary>
        /// Settings do metadados.
        /// </summary>
        protected Dictionary<Type, object> settings;

        /// <summary>
        /// MetaModel vinculado a este metadados. Normalmente o model é um singleton.
        /// </summary>
        /// <value>O MetaModel deste metadados.</value>
        public virtual MetaModel Model { get; }

        /// <summary>
        /// <para>
        ///     Metadados pai deste.
        /// </para>
        /// <para>
        ///     Pode ser o mesmo que o <see cref="Model"/>, quando o filho (este) é direto do <see cref="Model"/>.
        /// </para>
        /// </summary>
        /// <value>Um MetaBase que deu origem a este.</value>
        public virtual MetaBase Parent { get; }

        /// <summary>
        /// Construtor que inicializa <see cref="settings"/>.
        /// </summary>
        protected MetaBase(MetaModel metaModel, MetaBase parent)
        {
            Model = metaModel ?? throw new ArgumentNullException(nameof(metaModel));
            Parent = parent;
            settings = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Construtor interno para o <see cref="Model"/>.
        /// </summary>
        internal protected MetaBase()
        { 
            settings = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Verifica se existe algum setting configurado neste modelo.
        /// </summary>
        /// <typeparam name="TSettings">Tipo, classe, do Settings.</typeparam>
        /// <returns>Se existe o Settings para este modelo.</returns>
        public virtual bool HasSettings<TSettings>()
        {
            return settings.ContainsKey(typeof(TSettings));
        }

        /// <summary>
        /// Adiciona um Settings ao modelo.
        /// </summary>
        /// <typeparam name="TSettings">Settings a ser adicionado.</typeparam>
        /// <param name="value">Instância, objeto, do Settings.</param>
        /// <exception cref="ArgumentException">Se já existir um tipo, classe, de Settings no modelo.</exception>
        public virtual void AddSettings<TSettings>(TSettings value)
        {
            settings.Add(typeof(TSettings), value);
        }

        /// <summary>
        /// Obtém o Setting pelo tipo, classe.
        /// </summary>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings ou nulo.</returns>
        public virtual TSettings GetSettings<TSettings>()
        {
            return (TSettings)settings[typeof(TSettings)];
        }

        /// <summary>
        /// Tenta obtém o settings se ele existir.
        /// </summary>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <param name="settings">Saída como instância de Settings, ou default se não existir.</param>
        /// <returns>Verdadeiro se existir, falso caso contrário.</returns>
        public virtual bool TryGetSettings<TSettings>(out TSettings settings)
        {
            var result = HasSettings<TSettings>();
            settings = result ? GetSettings<TSettings>() : default;
            return result;
        }

        /// <summary>
        /// Obtém ou cria um Settings para este modelo.
        /// </summary>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings.</returns>
        public virtual TSettings GetOrAddSettings<TSettings>()
            where TSettings : new()
        {
            if (!HasSettings<TSettings>())
                AddSettings(new TSettings());

            return GetSettings<TSettings>();
        }

        /// <summary>
        /// Obtém ou cria um Settings para este modelo.
        /// </summary>
        /// <param name="factory">Função para criar um <typeparamref name="TSettings"/>.</param>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings.</returns>
        public virtual TSettings GetOrAddSettings<TSettings>(Func<TSettings> factory)
        {
            if (!HasSettings<TSettings>())
                AddSettings(factory());

            return GetSettings<TSettings>();
        }

        protected virtual TSettings CreateSettings<TSettings>()
            where TSettings : new()
        {
            return new TSettings();
        }
    }
}

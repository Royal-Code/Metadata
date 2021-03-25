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
        protected Dictionary<Type, object> settingsObjects;

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
        /// Construtor que inicializa <see cref="settingsObjects"/>.
        /// </summary>
        protected MetaBase(MetaModel metaModel, MetaBase parent)
        {
            Model = metaModel ?? throw new ArgumentNullException(nameof(metaModel));
            Parent = parent;
            settingsObjects = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Construtor interno para o <see cref="Model"/>.
        /// </summary>
        protected internal MetaBase()
        { 
            settingsObjects = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Verifica se existe algum setting configurado neste modelo.
        /// </summary>
        /// <typeparam name="TSettings">Tipo, classe, do Settings.</typeparam>
        /// <returns>Se existe o Settings para este modelo.</returns>
        public virtual bool HasSettings<TSettings>()
        {
            return settingsObjects.ContainsKey(typeof(TSettings));
        }

        /// <summary>
        /// Verifica se existe algum setting com nome específico configurado neste modelo.
        /// </summary>
        /// <param name="name">Nome do settings.</param>
        /// <typeparam name="TSettings">Tipo, classe, do Settings.</typeparam>
        /// <returns>Se existe o Settings com o nome para este modelo.</returns>
        public virtual bool HasNamedSettings<TSettings>(string name) 
        {
            if (settingsObjects.ContainsKey(typeof(NamedSettings<TSettings>)))
            {
                var named = (NamedSettings<TSettings>) settingsObjects[typeof(NamedSettings<TSettings>)];
                return named.HasSettings(name);
            }

            return false;
        }

        /// <summary>
        /// Adiciona um Settings ao modelo.
        /// </summary>
        /// <typeparam name="TSettings">Settings a ser adicionado.</typeparam>
        /// <param name="value">Instância, objeto, do Settings.</param>
        /// <exception cref="ArgumentException">Se já existir um tipo, classe, de Settings no modelo.</exception>
        public virtual void AddSettings<TSettings>(TSettings value)
        {
            settingsObjects.Add(typeof(TSettings), value);
        }

        /// <summary>
        /// Adiciona um Settings com um nome específico ao modelo.
        /// </summary>
        /// <param name="value">Instância, objeto, do Settings.</param>
        /// <param name="name">Nome do settings.</param>
        /// <typeparam name="TSettings">Settings a ser adicionado.</typeparam>
        /// <exception cref="ArgumentException">Se já existir um tipo, classe, de Settings com o mesmo nome no modelo.</exception>
        public virtual void AddNamedSettings<TSettings>(TSettings value, string name)
        {
            NamedSettings<TSettings> named;
            if (settingsObjects.ContainsKey(typeof(NamedSettings<TSettings>)))
            {
                named = (NamedSettings<TSettings>) settingsObjects[typeof(NamedSettings<TSettings>)];
            }
            else
            {
                named = new NamedSettings<TSettings>();
                settingsObjects.Add(typeof(NamedSettings<TSettings>), named);
            }

            named.AddSettings(value, name);
        }

        /// <summary>
        /// Obtém o Setting pelo tipo, classe.
        /// </summary>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings ou nulo.</returns>
        public virtual TSettings GetSettings<TSettings>()
        {
            return (TSettings)settingsObjects[typeof(TSettings)];
        }

        /// <summary>
        /// Obtém o Setting com um nome específico pelo tipo, classe.
        /// </summary>
        /// <param name="name">Nome do settings.</param>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings ou nulo.</returns>
        public virtual TSettings GetNamedSettings<TSettings>(string name)
        {
            if (settingsObjects.ContainsKey(typeof(NamedSettings<TSettings>)))
            {
                var named = (NamedSettings<TSettings>) settingsObjects[typeof(NamedSettings<TSettings>)];
                return named.GetSettings(name);
            }

            return default;
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
        /// Tenta obtém o settings com nome específico se ele existir.
        /// </summary>
        /// <param name="name">Nome do settings.</param>
        /// <param name="settings">Saída como instância de Settings, ou default se não existir.</param>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>Verdadeiro se existir, falso caso contrário.</returns>
        public virtual bool TryGetNamedSettings<TSettings>(string name, out TSettings settings)
        {
            var result = HasNamedSettings<TSettings>(name);
            settings = result ? GetNamedSettings<TSettings>(name) : default;
            return result;
        }

        /// <summary>
        /// Obtém ou cria um Settings para este modelo.
        /// </summary>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings.</returns>
        public virtual TSettings GetOrCreateSettings<TSettings>()
            where TSettings : class
        {
            if (!HasSettings<TSettings>())
                AddSettings(CreateSettings<TSettings>());

            return GetSettings<TSettings>();
        }

        /// <summary>
        /// Obtém ou cria um Settings com nome específico para este modelo.
        /// </summary>
        /// <param name="name">Nome do settings.</param>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings.</returns>
        public virtual TSettings GetOrCreateNamedSettings<TSettings>(string name)
            where TSettings : class
        {
            if (!HasNamedSettings<TSettings>(name))
                AddNamedSettings(CreateSettings<TSettings>(), name);

            return GetNamedSettings<TSettings>(name);
        }

        /// <summary>
        /// Obtém ou cria um Settings para este modelo.
        /// </summary>
        /// <param name="factory">Função para criar um <typeparamref name="TSettings"/>.</param>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings.</returns>
        public virtual TSettings GetOrCreateSettings<TSettings>(Func<TSettings> factory)
        {
            if (!HasSettings<TSettings>())
                AddSettings(factory());

            return GetSettings<TSettings>();
        }

        /// <summary>
        /// btém ou cria um Settings com nome específico para este modelo.
        /// </summary>
        /// <param name="name">Nome do settings.</param>
        /// <param name="factory">Função para criar um <typeparamref name="TSettings"/>.</param>
        /// <typeparam name="TSettings">Tipo, classe, de Settings a ser obtido.</typeparam>
        /// <returns>A instância de Settings.</returns>
        public virtual TSettings GetOrCreateNamedSettings<TSettings>(string name, Func<TSettings> factory)
        {
            if (!HasNamedSettings<TSettings>(name))
                AddNamedSettings(factory(), name);

            return GetNamedSettings<TSettings>(name);
        }

        protected virtual TSettings CreateSettings<TSettings>()
            where TSettings : class
        {
            return Model.TryActivateSettings<TSettings>(this);
        }

        private class NamedSettings<TSettings>
        {
            private readonly Dictionary<string, TSettings> settings = new();
            
            public bool HasSettings(string name)
            {
                return settings.ContainsKey(name);
            }

            public void AddSettings(TSettings value, string name)
            {
                settings.Add(name, value);
            }

            public TSettings GetSettings(string name)
            {
                return settings[name];
            }
        }
    }
}

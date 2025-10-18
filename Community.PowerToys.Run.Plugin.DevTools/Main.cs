using ManagedCommon;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.Community.PowerToys.Run.Plugin.DevTools
{
    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public class Main : IPlugin, IContextMenu, IDisposable
    {
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "2270582985BF420D9686F22A62911160";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "DevTools";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "A small developer toolbox.";

        private PluginInitContext Context { get; set; }

        private string IconPath { get; set; }

        private bool Disposed { get; set; }

        private List<IDataGenerator> Generators { get; } = [
            new HashDataGenerator(),
            new UuidDataGenerator(),
            new LoremDataGenerator(),
        ];

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            Logger.LogInfo($"Query: `{query.Search}`");
            var parts = query.Search.Split(Wox.Plugin.Query.TermSeparator, 2);

            var commandName = parts[0].ToLowerInvariant();
            var arguments = parts.Length > 1 ? parts[1] : string.Empty;

            foreach (var generator in Generators)
            {
                var values = generator.GenerateValues(commandName, arguments);

                if (values == null || values.Count == 0)
                {
                    continue;
                }

                return values.ConvertAll(value => new Result
                {
                    QueryTextDisplay = query.Search,
                    IcoPath = IconPath,
                    Title = value.Title ?? value.Value,
                    SubTitle = value.SubTitle,
                    Action = _ =>
                    {
                        Clipboard.SetDataObject(value.Value);
                        return true;
                    },
                });
            }

            return Recommand(query.ActionKeyword, commandName);
        }

        public List<Result> Recommand(string actionKeyword, string prefix)
        {
            List<Result> results = [];

            foreach (var generator in Generators)
            {
                var recommandations = generator.Recommand();

                if (recommandations == null || recommandations.Count == 0)
                {
                    continue;
                }

                results.AddRange(recommandations.ConvertAll(recommandation => new Result
                {
                    IcoPath = IconPath,
                    Title = recommandation.Title,
                    SubTitle = recommandation.SubTitle,
                    QueryTextDisplay = $"{recommandation.SubCommand} ",
                    Action = _ =>
                    {
                        Context.API.ChangeQuery($"{actionKeyword} {recommandation.SubCommand} ", true);
                        return false;
                    },
                }));
            }

            if (!string.IsNullOrWhiteSpace(prefix))
            {
                results.RemoveAll(result => !result.QueryTextDisplay.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            }

            results.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase));

            return results;
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());

            Logger.InitializeLogger("\\PowerToys Run\\Logs\\DevTools");
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult.ContextData is string search)
            {
                return
                [
                    new ContextMenuResult
                    {
                        PluginName = Name,
                        Title = "Copy to clipboard (Ctrl+C)",
                        FontFamily = "Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy
                        AcceleratorKey = Key.C,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            Clipboard.SetDataObject(search);
                            return true;
                        },
                    }
                ];
            }

            return [];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/devtools.light.png" : "Images/devtools.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);

    }
}

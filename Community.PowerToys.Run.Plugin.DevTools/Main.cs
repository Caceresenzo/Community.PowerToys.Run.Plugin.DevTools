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

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            Logger.LogInfo($"Query: `{query.Search}`");
            var parts = query.Search.Split(" ", 2);
            if (parts.Length != 2)
            {
                return [];
            }

            var algorithmName = parts[0].ToUpper();
            var content = parts[1];

            var hash = Hash(algorithmName, content);

            return
            [
                new Result
                {
                    QueryTextDisplay = hash,
                    IcoPath = IconPath,
                    Title = hash,
                    SubTitle = $"{algorithmName} Hash",
                    ToolTipData = new ToolTipData("Hashed from", content),
                    Action = _ =>
                    {
                        Clipboard.SetDataObject(hash);
                        return true;
                    },
                    ContextData = hash,
                }
            ];
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

        public static string Hash(string algorithmName, string input)
        {
            using System.Security.Cryptography.HashAlgorithm algorithm = algorithmName.ToUpper() switch
            {
                "MD5" => System.Security.Cryptography.MD5.Create(),
                "SHA1" => System.Security.Cryptography.SHA1.Create(),
                "SHA256" => System.Security.Cryptography.SHA256.Create(),
                "SHA384" => System.Security.Cryptography.SHA384.Create(),
                "SHA512" => System.Security.Cryptography.SHA512.Create(),
                _ => null,
            };

            if (algorithm == null)
            {
                return null;
            }

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = algorithm.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }

    }
}

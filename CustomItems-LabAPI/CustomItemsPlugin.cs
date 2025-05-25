using CustomItems.Core;
using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using System;
using System.ComponentModel;

namespace CustomItems;

public class CustomItemsPlugin : Plugin
{
    public static CustomItemsPlugin Instance { get; private set; }

    public override string Name => "CustomItems";

    public override string Description => "Adds support for custom items in the game.";

    public override string Author => "Bill";

    public override Version Version => new(0, 0, 1);

    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    public override LoadPriority Priority => LoadPriority.Highest;

    public CustomItemsConfig Config;
    private Harmony harmony;
    private Core.EventHandler Events { get; } = new();

    private bool _hasIncorrectSettings = false;
    public override void LoadConfigs()
    {
        base.LoadConfigs();

        _hasIncorrectSettings = !this.TryLoadConfig("config.yml", out Config);
    }

    public override void Enable()
    {
        Instance = this;

        if (_hasIncorrectSettings)
            Log.Error("CustomItems: Incorrect settings in config.yml. Please check the file.");

        harmony = new Harmony("bill.customitems-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        harmony.PatchAll();
        foreach (var a in harmony.GetPatchedMethods())
        {
            Log.Debug("Patched: " + a.Name);
        }

        CustomHandlersManager.RegisterEventsHandler(Events);

        if (Config.EnableExampleItems) API.CustomItems.RegisterAll();

        Log.Info("CustomItems plugin loaded successfully.");
    }

    public override void Disable()
    {
        if (Config.EnableExampleItems) API.CustomItems.UnregisterAll();
        harmony?.UnpatchAll(harmony?.Id);
        CustomHandlersManager.UnregisterEventsHandler(Events);
    }
}

public class CustomItemsConfig
{
    public bool Debug { get; set; } = true;

    [Description("If enabled, example items will be registered")]
    public bool EnableExampleItems { get; set; } = true;
}
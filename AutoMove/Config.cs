


namespace Ramune.AutoMove
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableMod { get; private set; }

        public static ConfigEntry<KeyboardShortcut> AutoMoveKeybind { get; private set; }

        public static ConfigEntry<bool> CancelOnBackward { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableMod = config.Bind("General", "Enable Mod", true, "If true, the mod will enable pressing the auto-move keybind to toggle an auto-move state. (default: true)");

            AutoMoveKeybind = config.Bind("General", "Auto-Move Keybind", new KeyboardShortcut(KeyCode.Mouse4), "The keybind used to toggle the auto-move state. (default: Mouse4)");

            CancelOnBackward = config.Bind("General", "Cancel On Moving Backward", true, "If true, cancels the auto-move state if you move backwards. (default: true)");

            ModSettingsManager.SetModDescription("AutoMove.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableMod));

            ModSettingsManager.AddOption(new KeyBindOption(AutoMoveKeybind));

            ModSettingsManager.AddOption(new CheckBoxOption(CancelOnBackward));
        }
    }
}
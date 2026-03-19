

using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace Ramune.CommandPauseRemade
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableSingleplayer{ get; private set; }
        public static ConfigEntry<bool> EnableMultiplayer { get; private set; }
        public static ConfigEntry<float> ProtectionDelay { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableSingleplayer = config.Bind("Singleplayer", "Enable Singleplayer Pausing", true, "If true, time will freeze while opening the Artifact of Command menu in singleplayer.");

            EnableMultiplayer = config.Bind("Multiplayer", "Enable Multiplayer Protection", true, "If true, players receive god mode and protection buffs while in the Artifact of Command menu in multiplayer.");

            ProtectionDelay = config.Bind("Multiplayer", "Protection Removal Delay", 0.35f, "The delay (in seconds) before protections are removed after closing the Command menu.");

            ModSettingsManager.SetModDescription("Pauses the game in singleplayer and provides protection for all players in multiplayer while using Artifact of Command menu.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableSingleplayer, true));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableMultiplayer, true));

            ModSettingsManager.AddOption(new StepSliderOption(ProtectionDelay, new StepSliderConfig
            {
                min = 0f,
                max = 10f,
                increment = 0.01f
            }));
        }
    }
}
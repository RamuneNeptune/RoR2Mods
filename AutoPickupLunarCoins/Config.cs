

namespace Ramune.AutoPickupLunarCoins
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableMod { get; private set; }

        public static ConfigEntry<int> CoinsToAward { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableMod = config.Bind("General", "Enable Mod", true, "If true, the mod will award lunar coins to all players when they are spawned. (default: true)");

            CoinsToAward = config.Bind("General", "Coins To Award", 1, "The amount of lunar coins to award to all players when they are spawned. (default: 1)");

            ModSettingsManager.SetModDescription("AutoPickupLunarCoins.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableMod));

            ModSettingsManager.AddOption(new IntSliderOption(CoinsToAward, new IntSliderConfig()
            {
                min = 0,
                max = 100,
            }));
        }
    }
}
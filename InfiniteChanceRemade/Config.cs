

namespace Ramune.InfiniteChanceRemade
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableCostMultiplier { get; private set; }

        public static ConfigEntry<float> CostMultiplier { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableCostMultiplier = config.Bind("General", "Enable Cost Multiplier", true, "If true, the Shrine of Chance cost multiplier will be used for custom price scaling. (default: true)");

            CostMultiplier = config.Bind("General", "Cost Multiplier", 1.4f, "The amount to multiply the shrine's cost by per purchase. (vanilla default: 1.4)");

            ModSettingsManager.SetModDescription("InfiniteChanceRemade.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCostMultiplier));

            ModSettingsManager.AddOption(new StepSliderOption(CostMultiplier, new StepSliderConfig
            {
                min = 0f,
                max = 10f,
                increment = 0.1f
            }));
        }
    }
}
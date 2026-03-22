

namespace Ramune.LocaterRemade
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableMod { get; private set; }

        public static ConfigEntry<bool> EnableDropShadow { get; private set; }

        public static ConfigEntry<bool> EnableLookAtRequired { get; private set; }

        public static ConfigEntry<float> LookAtFloat { get; private set; }

        public static ConfigEntry<float> SizeMultiplier { get; private set; }

        public static ConfigEntry<bool> EnableCategoryTeleporterInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryGenericPickupController { get; private set; }

        public static ConfigEntry<bool> EnableCategoryGenericInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryPurchaseInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryBarrelInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryDroneCombinerController { get; private set; }

        public static ConfigEntry<bool> EnableCategoryScrapperController { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableMod = config.Bind("General", "Enable Mod", true, "If true, the mod will be enabled. (default: true)");

            EnableDropShadow = config.Bind("General", "Enable Drop Shadow", true, "If true, labels will have a drop shadow. (default: true)");

            EnableLookAtRequired = config.Bind("General", "Enable Look At Required", true, "If true, you must look at an icon to display its label. (default: true)");

            LookAtFloat = config.Bind("General", "Look At Float", 0.995f, "Idk what to call this one. (default: 0.995)");

            SizeMultiplier = config.Bind("General", "Size Multiplier", 1f, "It do be multiplyin. (default: 1.0)");

            EnableCategoryTeleporterInteraction = config.Bind("Categories", "TeleporterInteraction", true, "If true, TeleporterInteractions will be tracked and have labels. (default: true)");

            EnableCategoryGenericPickupController = config.Bind("Categories", "GenericPickupController", true, "If true, GenericPickupControllers will be tracked and have labels. (default: true)");

            EnableCategoryGenericInteraction = config.Bind("Categories", "GenericInteraction", true, "If true, GenericInteractions will be tracked and have labels. (default: true)");

            EnableCategoryPurchaseInteraction = config.Bind("Categories", "PurchaseInteraction", true, "If true, PurchaseInteractions will be tracked and have labels. (default: true)");

            EnableCategoryBarrelInteraction = config.Bind("Categories", "BarrelInteraction", true, "If true, BarrelInteractions will be tracked and have labels. (default: true)");

            EnableCategoryDroneCombinerController = config.Bind("Categories", "DroneCombinerController", true, "If true, DroneCombinerControllers will be tracked and have labels. (default: true)");

            EnableCategoryScrapperController = config.Bind("Categories", "ScrapperController", true, "If true, ScrapperControllers will be tracked and have labels. (default: true)");

            ModSettingsManager.SetModDescription("Locater.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableMod));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableDropShadow));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableLookAtRequired));

            ModSettingsManager.AddOption(new StepSliderOption(LookAtFloat, new StepSliderConfig
            {
                min = 0f,
                max = 1f,
                increment = 0.001f
            }));

            ModSettingsManager.AddOption(new StepSliderOption(SizeMultiplier, new StepSliderConfig
            {
                min = 0f,
                max = 5f,
                increment = 0.1f
            }));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryTeleporterInteraction));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryGenericPickupController));    

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryGenericInteraction));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryPurchaseInteraction));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryBarrelInteraction));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryDroneCombinerController));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryScrapperController));
        }
    }
}
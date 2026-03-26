

namespace Ramune.LocaterRemade
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableMod { get; private set; }

        public static ConfigEntry<bool> EnableDropShadow { get; private set; }

        public static ConfigEntry<bool> EnableBazaarCheck { get; private set; }

        public static ConfigEntry<bool> EnableLookAtRequired { get; private set; }

        public static ConfigEntry<float> LookAtFloat { get; private set; }

        public static ConfigEntry<bool> EnableMaxDistance { get; private set; }

        public static ConfigEntry<float> MaxDistance { get; private set; }

        public static ConfigEntry<float> GlobalScale { get; private set; }

        public static ConfigEntry<float> GlobalIconScale { get; private set; }

        public static ConfigEntry<float> GlobalLabelScale { get; private set; }

        public static ConfigEntry<bool> EnableCategoryTeleporterInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryGenericPickupController { get; private set; }

        public static ConfigEntry<bool> EnableCategoryPickupPickerController { get; private set; }

        public static ConfigEntry<bool> EnableCategoryGenericInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryPurchaseInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryBarrelInteraction { get; private set; }

        public static ConfigEntry<bool> EnableCategoryDroneCombinerController { get; private set; }

        public static ConfigEntry<bool> EnableCategoryScrapperController { get; private set; }

        public sealed class TrackedInstanceConfig
        {
            public ConfigEntry<string> Label { get; set; }
            public ConfigEntry<bool> Enabled { get; set; }
            public ConfigEntry<Color> Color { get; set; }
            public ConfigEntry<bool> ColorEnabled { get; set; }
            public ConfigEntry<float> IconScale { get; set; }
            public ConfigEntry<float> LabelScale { get; set; }
        }

        public static readonly Dictionary<string, TrackedInstanceConfig> TrackedInstanceCategoryConfigs = [];

        public static readonly Dictionary<string, TrackedInstanceConfig> TrackedInstanceConfigs = [];

        public static Dictionary<string, string> TrackedInstanceCategoryLookup { get; } = [];


        public static void Init(ConfigFile config)
        {
            EnableMod = config.Bind("General", "Enable Mod", true, "If true, the mod will be enabled. (default: true)");
            EnableDropShadow = config.Bind("General", "Enable Drop Shadow", true, "If true, labels will have a drop shadow. (default: true)");
            EnableBazaarCheck = config.Bind("General", "Enable Bazaar Check", true, "If true, only tracks portals in the Bazaar. (default: true)");
            EnableLookAtRequired = config.Bind("General", "Enable Look At Required", true, "If true, you must look at an icon to display its label. (default: true)");
            LookAtFloat = config.Bind("General", "Look At Float", 0.995f, "Idk what to call this one. (default: 0.995)");
            EnableMaxDistance = config.Bind("General", "Enable Max Distance", false, "If true, enable calculating max distance to filter objects to track. (default: false)");
            MaxDistance = config.Bind("General", "Max Distance", 1000f, "The maximum distance to track objects at. (default: 1000m)");
            GlobalScale = config.Bind("General", "Global Scale", 1f, "Global scale for all tracked objects. (default: 1.0)");
            GlobalIconScale = config.Bind("General", "Global Icon Scale", 1f, "Global icon scale for all tracked objects. (default: 1.0)");
            GlobalLabelScale = config.Bind("General", "Global Label Scale", 1f, "Global label scale for all tracked objects. (default: 1.0)");

            EnableCategoryTeleporterInteraction = config.Bind("General", "Track TeleporterInteractions", true, "If true, TeleporterInteraction's will be tracked and have labels/icons. (default: true)");
            EnableCategoryGenericPickupController = config.Bind("General", "Track GenericPickupControllers", true, "If true, GenericPickupController's will be tracked and have labels/icons. (default: true)");
            EnableCategoryPickupPickerController = config.Bind("General", "Track PickupPickerControllers", true, "If true, PickupPickerController's will be tracked and have labels/icons. (default: true)");
            EnableCategoryGenericInteraction = config.Bind("General", "Track GenericInteractions", true, "If true, GenericInteraction's will be tracked and have labels/icons. (default: true)");
            EnableCategoryPurchaseInteraction = config.Bind("General", "Track PurchaseInteractions", true, "If true, PurchaseInteraction's will be tracked and have labels/icons. (default: true)");
            EnableCategoryBarrelInteraction = config.Bind("General", "Track BarrelInteractions", true, "If true, BarrelInteraction's will be tracked and have labels/icons. (default: true)");
            EnableCategoryDroneCombinerController = config.Bind("General", "Track DroneCombinerControllers", true, "If true, DroneCombinerController's will be tracked and have labels/icons. (default: true)");
            EnableCategoryScrapperController = config.Bind("General", "Track ScrapperControllers", true, "If true, ScrapperController's will be tracked and have labels/icons. (default: true)");

            ModSettingsManager.SetModDescription("LocaterRemade.");
            ModSettingsManager.AddOption(new CheckBoxOption(EnableMod));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableDropShadow));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableBazaarCheck));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableLookAtRequired));
            ModSettingsManager.AddOption(new StepSliderOption(LookAtFloat, new StepSliderConfig
            {
                min = 0f,
                max = 1f,
                increment = 0.001f
            }));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableMaxDistance));
            ModSettingsManager.AddOption(new StepSliderOption(MaxDistance, new StepSliderConfig
            {
                min = 0f,
                max = 5000f,
                increment = 1f
            }));
            ModSettingsManager.AddOption(new StepSliderOption(GlobalScale, new StepSliderConfig
            {
                min = 0f,
                max = 5f,
                increment = 0.1f
            }));
            ModSettingsManager.AddOption(new StepSliderOption(GlobalIconScale, new StepSliderConfig
            {
                min = 0f,
                max = 5f,
                increment = 0.1f
            }));
            ModSettingsManager.AddOption(new StepSliderOption(GlobalLabelScale, new StepSliderConfig
            {
                min = 0f,
                max = 5f,
                increment = 0.1f
            }));

            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryTeleporterInteraction));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryGenericPickupController));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryPickupPickerController));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryGenericInteraction));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryPurchaseInteraction));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryBarrelInteraction));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryDroneCombinerController));
            ModSettingsManager.AddOption(new CheckBoxOption(EnableCategoryScrapperController));

            TrackedInstanceCategoryLookup.Clear();
            TrackedInstanceConfigs.Clear();


            var path = Path.Combine(Paths.ConfigPath, "Ramune.LocaterRemade.Categories.json");

            if(!File.Exists(path))
            {
                var defaultData = new Dictionary<string, List<string>>
                {
                    ["Objectives"] = 
                    [
                        "Teleporter",
                        "Primordial Teleporter",
                        "Cell Vent"
                    ],

                    ["Chests"] =
                    [
                        "Chest",
                        "Chest - Damage",
                        "Chest - Healing",
                        "Chest - Quality",
                        "Chest - Utility",
                        "Large Chest",
                        "Large Chest - Damage",
                        "Large Chest - Healing",
                        "Large Chest - Quality",
                        "Large Chest - Utility",
                        "Adaptive Chest",
                        "Cloaked Chest",
                        "Legendary Chest",
                        "Rusty Lockbox",
                        "Encrusted Lockbox",
                        "Scavenger's Sack"
                    ],

                    ["Barrels"] = 
                    [
                        "Barrel", 
                        "Equipment Barrel"
                    ],

                    ["Shrines"] =
                    [
                        "Halcyon Shrine",
                        "Shrine of Blood",
                        "Shrine of Combat",
                        "Shrine of the Mountain",
                        "Shrine of Chance",
                        "Shrine of the Woods",
                        "Shrine of Order",
                        "Shrine of Shaping",
                        "Collective Shrine of Combat",
                        "Altar of Gold",
                        "Newt Altar"
                    ],

                    ["Portals"] =
                    [
                        "Mainline Portal",
                        "Gold Portal",
                        "Celestial Portal",
                        "Blue Portal",
                        "Infinite Portal",
                        "Deep Void Portal",
                        "Void Portal",
                        "Portal?",
                        "Destination Portal",
                        "Green Portal",
                        "Encrypted Portal",
                        "Virtual Portal",
                        "Null Portal"
                    ],

                    ["Shops"] =
                    [
                        "Multishop Terminal",
                        "Large Multishop Terminal",
                        "Equipment Shop Terminal",
                        "Shipping Terminal",
                        "Triple Drone Shop"
                    ],

                    ["Machines"] =
                    [
                        "3D Printer",
                        "3D Printer - Quality",
                        "Overgrown 3D Printer",
                        "Large 3D Printer",
                        "Mili-Tech Printer",
                        "Mili-Tech Printer - Quality",
                        "Temporary Item Distributor",
                        "Scrapper",
                        "Drone Scrapper",
                        "Cleansing Pool"
                    ],

                    ["Drones"] =
                    [
                        "Broken Healing Drone",
                        "Broken Gunner Drone",
                        "Broken Missile Drone",
                        "Broken Incinerator Drone",
                        "Broken Emergency Drone",
                        "Broken Equipment Drone",
                        "Broken Transport Drone",
                        "Broken Junk Drone",
                        "Broken TC-280",
                        "Broken Bombardment Drone",
                        "Broken Freeze Drone",
                        "Broken Jailer Drone",
                        "Broken Barrier Drone",
                        "Cleanup Drone",
                        "Drone Combiner Station",
                        "Drone Upgrade Station",
                        "Broken Gunner Turret"
                    ],

                    ["Essence"] =
                    [
                        "White Command Essence",
                        "Green Command Essence",
                        "Red Command Essence",
                        "Yellow Command Essence",
                        "Blue Command Essence",
                        "Orange Command Essence",
                        "Pink Command Essence"
                    ],

                    ["Void"] =
                    [
                        "Void Cradle",
                        "Void Potential",
                        "Deep Void Signal"
                    ],

                    ["Braziers"] =
                    [
                        "Buff Brazier (Soul Linked)",
                        "Buff Brazier (Cripple)",
                        "Buff Brazier (Mercenary Expose)",
                        "Buff Brazier (Jade Elephant)",
                        "Buff Brazier (Warcry)",
                        "Buff Brazier (Double XP and Double Gold)",
                        "Buff Brazier (Super Leech)",
                        "Buff Brazier (80 Percent Slowdown)",
                        "Buff Brazier (Brainstalks)"
                    ],

                    ["Misc"] =
                    [
                        "Lunar Pod",
                        "Lunar Coin",
                        "Radio Scanner",
                        "Halcyon Beacon",
                        "Assessment Focus",
                        "Defunct Unit",
                        "Stalk"
                    ]
                };

                File.WriteAllText(path, JsonConvert.SerializeObject(defaultData, Formatting.Indented));
            }

            var TrackedInstanceConfigCategories = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText(path));

            foreach(var categoryPair in TrackedInstanceConfigCategories)
            {
                var categoryName = categoryPair.Key;

                var trackedConfig1 = new TrackedInstanceConfig
                {
                    Enabled = config.Bind(categoryName, $"Category {categoryName} Enabled", true, $"If true, all enabled objects in the \"{categoryName}\" category will be tracked. (default: true)"),
                    Color = config.Bind(categoryName, $"Category {categoryName} Color", Color.white, $"Color for all enabled objects in the \"{categoryName}\" category. (default: white)"),
                    ColorEnabled = config.Bind(categoryName, $"Category {categoryName} Color Enabled", false, $"If true, all enabled objects in the \"{categoryName}\" category will use a custom color instead of the default highlight color. (default: false)"),
                    IconScale = config.Bind(categoryName, $"Category {categoryName} Icon Scale", 1f, $"Icon scale multiplier for all enabled objects in the \"{categoryName}\" category. (default: 1.0)"),
                    LabelScale = config.Bind(categoryName, $"Category {categoryName} Label Scale", 1f, $"Label scale multiplier for all enabled objects in the \"{categoryName}\" category. (default: 1.0)")
                };

                TrackedInstanceCategoryConfigs[categoryName] = trackedConfig1;

                ModSettingsManager.AddOption(new CheckBoxOption(trackedConfig1.Enabled));
                ModSettingsManager.AddOption(new ColorOption(trackedConfig1.Color));
                ModSettingsManager.AddOption(new CheckBoxOption(trackedConfig1.ColorEnabled));
                ModSettingsManager.AddOption(new StepSliderOption(trackedConfig1.IconScale, new StepSliderConfig
                {
                    min = 0.1f,
                    max = 3f,
                    increment = 0.1f
                }));
                ModSettingsManager.AddOption(new StepSliderOption(trackedConfig1.LabelScale, new StepSliderConfig
                {
                    min = 0.1f,
                    max = 3f,
                    increment = 0.1f
                }));

                foreach(var objectName in categoryPair.Value)
                {
                    TrackedInstanceCategoryLookup[objectName] = categoryName;

                    var sanitizedName = objectName.Sanitize();

                    var trackedConfig2 = new TrackedInstanceConfig
                    {
                        Enabled = config.Bind(categoryName, $"{sanitizedName} Enabled", true, $"If true, \"{objectName}\" will be tracked. (default: true)"),
                        Label = config.Bind(categoryName, $"{sanitizedName} Label", objectName, $"Label for \"{objectName}\". (default: {objectName})"),
                        Color = config.Bind(categoryName, $"{sanitizedName} Color", Color.white, $"Color for \"{objectName}\". (default: white)"),
                        ColorEnabled = config.Bind(categoryName, $"{sanitizedName} Color Enabled", false, $"If true, \"{objectName}\" will use a custom color instead of the default highlight color. (default: false)"),
                        IconScale = config.Bind(categoryName, $"{sanitizedName} Icon Scale", 1f, $"Icon scale multiplier for \"{objectName}\". (default: 1.0)"),
                        LabelScale = config.Bind(categoryName, $"{sanitizedName} Label Scale", 1f, $"Label scale multiplier for \"{objectName}\". (default: 1.0)")
                    };

                    TrackedInstanceConfigs[objectName] = trackedConfig2;

                    ModSettingsManager.AddOption(new StringInputFieldOption(trackedConfig2.Label));
                    ModSettingsManager.AddOption(new CheckBoxOption(trackedConfig2.Enabled));
                    ModSettingsManager.AddOption(new ColorOption(trackedConfig2.Color));
                    ModSettingsManager.AddOption(new CheckBoxOption(trackedConfig2.ColorEnabled));
                    ModSettingsManager.AddOption(new StepSliderOption(trackedConfig2.IconScale, new StepSliderConfig
                    {
                        min = 0.1f,
                        max = 3f,
                        increment = 0.1f
                    }));
                    ModSettingsManager.AddOption(new StepSliderOption(trackedConfig2.LabelScale, new StepSliderConfig
                    {
                        min = 0.1f,
                        max = 3f,
                        increment = 0.1f
                    }));
                }
            }
        }


        public static string Sanitize(this string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return "Unnamed";

            name = Regex.Replace(name, "<.*?>", "").Trim();

            var sb = new System.Text.StringBuilder(name.Length);

            foreach(var c in name)
            {
                if(char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_')
                    sb.Append(c);
            }

            var result = sb.ToString().Trim();

            return string.IsNullOrWhiteSpace(result) ? "Unnamed" : result;
        }
    }
}
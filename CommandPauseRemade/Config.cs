

namespace Ramune.CommandPauseRemade
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableSingleplayer{ get; private set; }
        public static ConfigEntry<bool> EnableMultiplayer { get; private set; }
        public static ConfigEntry<float> ProtectionDelay { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableSingleplayer = config.Bind("Singleplayer", "Enable Singleplayer Pausing", true, "If true, time will freeze while opening the Artifact of Command menu in singleplayer. (default: true)");

            EnableMultiplayer = config.Bind("Multiplayer", "Enable Multiplayer Protection", true, "If true, players receive protection while in the Artifact of Command menu in multiplayer. (default: true)");

            ProtectionDelay = config.Bind("Multiplayer", "Protection Removal Delay", 0.85f, "The delay (in seconds) before protections are removed after closing the Command menu. (default: 0.85)");

            var iconPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ModConfig).Assembly.Location), "icon.png");

            if (File.Exists(iconPath))
            {
                var iconTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                if(iconTexture.LoadImage(File.ReadAllBytes(iconPath)))
                {
                    iconTexture.filterMode = FilterMode.Point;
                    iconTexture.Apply();

                    var iconSprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));
                    ModSettingsManager.SetModIcon(iconSprite);
                }
            }

            ModSettingsManager.SetModDescription("While opening an Artifact of Command menu, pauses the game in Singeplayer, and provides protection to players in Multiplayer (Required host-only for MP)");

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
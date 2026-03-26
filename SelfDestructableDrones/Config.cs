

namespace Ramune.SelfDestructableDrones
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableChatMessage { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableChatMessage = config.Bind("General", "Enable Chat Message", true, "If true and hosting, broadcasts a message to chat when a player self destructs their drone. (default: false)");

            ModSettingsManager.SetModDescription("SelfDestructableDrones.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableChatMessage));
        }
    }
}
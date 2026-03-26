

namespace Ramune.AutoExitEscapePod
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableChatMessage { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableChatMessage = config.Bind("General", "Enable Chat Message", true, "If true, locally sends a message to your chat indicating you have been freed from your pod. (default: true)");

            ModSettingsManager.SetModDescription("AutoExitEscapePod.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableChatMessage));
        }
    }
}
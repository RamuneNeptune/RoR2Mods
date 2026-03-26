

namespace Ramune.UnReadyUp
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> EnableChatMessage { get; private set; }


        public static void Init(ConfigFile config)
        {
            EnableChatMessage = config.Bind("General", "Enable Chat Message", false, "If true, broadcasts a message to chat to debug which VoteControllers are being affected and when. (default: false)");

            ModSettingsManager.SetModDescription("UnReadyUp.");

            ModSettingsManager.AddOption(new CheckBoxOption(EnableChatMessage));
        }
    }
}
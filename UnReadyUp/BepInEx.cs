

namespace Ramune.UnReadyUp
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class UnReadyUp : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.UnReadyUp";
        public const string PluginVersion = "1.0.0";
        public static readonly Harmony Harmony = new(PluginGUID);

        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);
            Harmony.PatchAll();
        }
    }


    [HarmonyPatch(typeof(VoteController))]
    public static class VoteControllerPatch
    {
        [HarmonyPatch(nameof(VoteController.ReceiveUserVote)), HarmonyPrefix]
        public static void ReceiveUserVote(VoteController __instance, NetworkUser networkUser)
        {
            if(!NetworkServer.active)
                return;
            
            __instance.canChangeVote = true;
            __instance.canRevokeVote = true;
            
            if(ModConfig.EnableChatMessage.Value)
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = $"<color=#ffdf9a>{(!string.IsNullOrEmpty(networkUser.userName) ? networkUser.userName : "N/A")}:</color> Allowing un-vote ({(__instance.gameObject && !string.IsNullOrEmpty(__instance.gameObject.name) ? __instance.gameObject.name : "N/A")}" });
        }
    }


    [HarmonyPatch(typeof(CharacterSelectController))]
    public static class CharacterSelectControllerPatch
    {
        [HarmonyPatch(nameof(CharacterSelectController.Awake)), HarmonyPostfix]
        public static void Awake(CharacterSelectController __instance)
        {
            if(RoR2Application.isInSinglePlayer || !__instance.unreadyButton)
                return;

            __instance.unreadyButton.onClick.AddListener(__instance.ClientSetUnready);
        }


        [HarmonyPatch(nameof(CharacterSelectController.Update)), HarmonyPostfix]
        public static void Update(CharacterSelectController __instance)
        {
            if(RoR2Application.isInSinglePlayer || !__instance.unreadyButton || !__instance.IsClientReady()) 
                return;

            __instance.unreadyButton.gameObject.SetActive(true);
            __instance.unreadyButton.interactable = true;
            __instance.unreadyButton.enabled = true;
        }
    }
}
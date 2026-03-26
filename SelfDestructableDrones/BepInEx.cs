

namespace Ramune.SelfDestructableDrones
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class SelfDestructableDrones : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.SelfDestructableDrones";
        public const string PluginVersion = "1.0.0";
        public static readonly Harmony harmony = new(PluginGUID);

        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);
            NetworkingAPI.RegisterMessageType<ImDroningAndWantToDie>();
            harmony.PatchAll();
        }
    }


    [HarmonyPatch(typeof(EquipmentSlot))]
    public static class EquipmentSlotPatches
    {
        // Host
        [HarmonyPatch(nameof(EquipmentSlot.ExecuteIfReady)), HarmonyPrefix]
        public static bool ExecuteIfReady(EquipmentSlot __instance, ref bool __result)
        {
            if(!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Boolean RoR2.EquipmentSlot::ExecuteIfReady()' called on client");
                __result = false;
                return false;
            }

            var body = __instance.characterBody;

            if(!body || !body.IsDrone || !body.master || !body.master.lostBodyToDeath || !body.healthComponent || !body.healthComponent.alive)
                return true;
            
            body.healthComponent.Suicide();

            if(ModConfig.EnableChatMessage.Value)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = $"<color=#ff3a00>{(body.master != null && body.master.playerCharacterMasterController != null && body.master.playerCharacterMasterController.networkUser != null && !string.IsNullOrEmpty(body.master.playerCharacterMasterController.networkUser.userName) ? body.master.playerCharacterMasterController.networkUser.userName : "A Drone")} has self destructed!</color>"
                });
            }

            __result = false;
            return false;
        }

        // Client
        [HarmonyPatch(nameof(EquipmentSlot.CallCmdExecuteIfReady)), HarmonyPrefix]
        public static bool CallCmdExecuteIfReady(EquipmentSlot __instance)
        {
            if(NetworkServer.active || !NetworkClient.active)
            {
                Debug.LogError("Command function CmdExecuteIfReady called on server.");
                return false;
            }

            var body = __instance.characterBody;

            if(!body || !body.IsDrone || !body.master || !body.master.lostBodyToDeath || !body.healthComponent || !body.healthComponent.alive)
                return true;

            new ImDroningAndWantToDie(body.netId).Send(NetworkDestination.Server);
            return false;
        }
    }


    public struct ImDroningAndWantToDie(NetworkInstanceId netId) : INetMessage
    {
        public NetworkInstanceId netId = netId;

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
        }

        public void OnReceived()
        {
            if(!NetworkServer.active)
                return;

            var droneBodyObject = Util.FindNetworkObject(netId);
                
            if(!droneBodyObject || !droneBodyObject.TryGetComponent<CharacterBody>(out var droneBody) || !droneBody.healthComponent || !droneBody.healthComponent.alive)
                return;

            droneBody.healthComponent.Suicide();

            if(ModConfig.EnableChatMessage.Value)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = $"<color=#ff3a00>{(droneBody.master != null && droneBody.master.playerCharacterMasterController != null && droneBody.master.playerCharacterMasterController.networkUser != null && !string.IsNullOrEmpty(droneBody.master.playerCharacterMasterController.networkUser.userName) ? droneBody.master.playerCharacterMasterController.networkUser.userName : "A Drone")} has self destructed!</color>"
                });
            }
        }
    }
}
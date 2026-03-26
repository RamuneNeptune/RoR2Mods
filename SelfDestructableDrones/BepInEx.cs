

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

        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);
            NetworkingAPI.RegisterMessageType<ImDroningAndWantToDie>();

            On.RoR2.PlayerCharacterMasterController.PollButtonInput += (orig, self) =>
            {
                orig(self);

                if(!self || !self.hasAuthority)
                    return;

                var master = self.master;

                if(!master || !master.hasBody || !master.lostBodyToDeath)
                    return;

                var body = self.body ?? self.master.GetBody();

                if(!body || !body.IsDrone || !body.healthComponent || !body.healthComponent.alive)
                    return;

                if(self.bodyInputs.activateEquipment.justPressed != true)
                    return;

                if(NetworkServer.active)
                {
                    body.healthComponent.Suicide();

                    if(ModConfig.EnableChatMessage.Value)
                    {
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = $"<color=#ff3a00>{(self.networkUser != null && !string.IsNullOrEmpty(self.networkUser.userName) ? self.networkUser.userName : "A Drone")} has self destructed!</color>"
                        });
                    }
                }
                else new ImDroningAndWantToDie(body.netId).Send(NetworkDestination.Server);
            };
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
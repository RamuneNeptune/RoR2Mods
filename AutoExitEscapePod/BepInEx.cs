

namespace Ramune.AutoExitEscapePod
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ExamplePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.AutoExitEscapePod";
        public const string PluginVersion = "1.0.0";

        public bool hasEjected = false;

        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);

            Stage.onStageStartGlobal += (self) =>
            {
                hasEjected = false;
            };

            On.EntityStates.SurvivorPod.Landed.FixedUpdate += (orig, self) =>
            {
                orig(self);

                if(hasEjected || self.fixedAge <= 0f || self.outer.nextState != null) 
                    return;

                var survivorPodController = self.survivorPodController;

                if(!survivorPodController)
                    return;

                var seat = survivorPodController.vehicleSeat;

                if(!seat)
                    return;

                var passengerBody = seat.currentPassengerBody;

                if(!passengerBody || !passengerBody.hasEffectiveAuthority)
                    return;
  
                if(NetworkServer.active)
                {
                    seat.EjectPassenger();
                }
                else
                {
                    passengerBody.CallCmdRequestVehicleEjection();
                }

                hasEjected = true;

                if(ModConfig.EnableChatMessage.Value)
                {
                    Chat.AddMessage($"<color=#e5c962>{passengerBody.GetUserName()} is free!</color>");
                }
            };
        }
    }
}
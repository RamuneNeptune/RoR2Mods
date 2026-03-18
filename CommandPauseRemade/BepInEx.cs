

namespace Ramune.CommandPauseRemade
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class CommandPauseRemade : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.CommandPauseRemade";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            Log.Init(Logger);

            Stage.onStageStartGlobal += (self) => 
            {
                CommandPauseController.masterSessionIds.Clear();
            };

            On.RoR2.PickupPickerController.OnDisplayBegin += (orig, self, promptController, localUser, cameraRigController) =>
            {
                orig(self, promptController, localUser, cameraRigController);

                if(RoR2Application.isInSinglePlayer) // handle Singeplayer
                    Time.timeScale = 0f;
            };

            On.RoR2.PickupPickerController.OnDisplayEnd += (orig, self, promptController, localUser, cameraRigController) =>
            {
                orig(self, promptController, localUser, cameraRigController);

                if(RoR2Application.isInSinglePlayer) // handle Singeplayer
                    Time.timeScale = 1f;
            };

            On.RoR2.PickupPickerController.Awake += (orig, self) =>
            {
                orig(self);

                if(!NetworkServer.active)
                    return;

                var commandPauseController = self.gameObject.AddComponent<CommandPauseController>(); // handle Multiplayer
                commandPauseController.networkUIPromptController = self.networkUIPromptController;
            };
        }
    }
}


namespace Ramune.AutoMove
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class AutoMove : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.AutoMove";
        public const string PluginVersion = "1.0.0";

        public bool IsAutoMoveEnabled;

        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);
            
            On.RoR2.PlayerCharacterMasterController.Update += (orig, self) => {
                orig(self);

                if(!ModConfig.EnableMod.Value || !self.networkUser || !self.networkUser.isLocalPlayer)
                    return;

                if(Input.GetKeyDown(ModConfig.AutoMoveKeybind.Value.MainKey))
                    IsAutoMoveEnabled = !IsAutoMoveEnabled;

                if(!IsAutoMoveEnabled)
                    return;

                var master = self.master;

                if(!master || !master.hasBody)
                    return;

                var body = master.GetBody();
                var inputBank = body.inputBank;

                if(!inputBank)
                    return;

                if(ModConfig.CancelOnBackward.Value && inputBank.rawMoveData.y < -0.5f)
                {
                    IsAutoMoveEnabled = false;
                    return;
                }

                var aimDirection = inputBank.aimDirection;

                if(!body.isFlying)
                    aimDirection.y = 0;

                var combined = inputBank.moveVector + aimDirection.normalized;

                inputBank.moveVector = combined.normalized;
            };
        }
    }
}
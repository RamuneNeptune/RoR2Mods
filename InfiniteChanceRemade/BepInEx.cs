

namespace Ramune.InfiniteChanceRemade
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class InfiniteChanceRemade : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.InfiniteChanceRemade";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);

            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, interactor) =>
            {
                orig(self, interactor);

                if(!NetworkServer.active)
                    return;

                self.successfulPurchaseCount = 0;
                self.waitingForRefresh = false;
                self.refreshTimer = 0f;
                
                if(self.purchaseInteraction)
                {
                    self.purchaseInteraction.Networkcost = Mathf.FloorToInt(self.purchaseInteraction.cost * (ModConfig.EnableCostMultiplier.Value ? ModConfig.CostMultiplier.Value : self.costMultiplierPerPurchase));
                    self.purchaseInteraction.SetAvailableTrue();
                }

                if(self.symbolTransform != null && self.symbolTransform.gameObject != null)
                    self.symbolTransform.gameObject.SetActive(true);

                self.CallRpcSetPingable(true);
            };

            On.RoR2.PurchaseInteraction.SetAvailable += (orig, self, newAvailable) =>
            {
                if(self.displayNameToken == "SHRINE_CHANCE_NAME") 
                    newAvailable = true;

                orig(self, newAvailable);
            };
        }
    }
}
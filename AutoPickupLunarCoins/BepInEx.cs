

namespace Ramune.AutoPickupLunarCoins
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class ExamplePlugin : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.AutoPickupLunarCoins";
        public const string PluginVersion = "1.0.0";

        public static PickupIndex LunarCoinIndex;

        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);

            RoR2Application.onLoad += () =>
            {
                LunarCoinIndex = PickupCatalog.FindPickupIndex("LunarCoin.Coin0");
            };

            On.RoR2.GenericPickupController.CreatePickup += (orig, ref createPickupInfo) =>
            {
                if(!NetworkServer.active || !ModConfig.EnableMod.Value || createPickupInfo.pickup.pickupIndex != LunarCoinIndex)
                    return orig(ref createPickupInfo);

                foreach(var player in PlayerCharacterMasterController.instances)
                {
                    if(!player)
                        continue;

                    var networkUser = player.networkUser;

                    if(!networkUser)
                        continue;

                    networkUser.AwardLunarCoins(Convert.ToUInt32(ModConfig.CoinsToAward.Value));
                }

                return null;
            };
        }
    }
}
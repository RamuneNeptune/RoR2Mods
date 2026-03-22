

using UnityEngine.Analytics;

namespace Ramune.LocaterRemade
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class LocaterRemade : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.LocaterRemade";
        public const string PluginVersion = "1.0.0";

        public Camera camera;
        public readonly Dictionary<GameObject, string> DisplayNameCache = [];
        public readonly Dictionary<Sprite, Texture2D> SpriteCache = [];


        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);

            On.RoR2.PickupPickerController.OnEnable += (orig, self) =>
            {
                orig(self);

                if(self.GetComponentInParent<ScrapperController>() is var scrapper)
                    InstanceTracker.Add(scrapper);
            };

            On.RoR2.PickupPickerController.OnDisable += (orig, self) =>
            {
                if(self.GetComponentInParent<ScrapperController>() is var scrapper)
                    InstanceTracker.Remove(scrapper);

                orig(self);
            };

            Stage.onStageStartGlobal += (stage) =>
            {
                DisplayNameCache.Clear();
                SpriteCache.Clear();
            };
        }


        public void OnGUI()
        {
            if(!ModConfig.EnableMod.Value || !Run.instance || PauseManager.isPaused)
                return;

            if(!camera)
                camera = Camera.main;

            if(!camera)
                return;

            var localUser = LocalUserManager.GetFirstLocalUser();

            if(localUser?.eventSystem && localUser.eventSystem.isCursorVisible)
                return;

            // Teleporters
            if(ModConfig.EnableCategoryTeleporterInteraction.Value)
            {
                foreach(var teleporter in InstanceTracker.GetInstancesList<TeleporterInteraction>())
                {
                    if(teleporter)
                        ProcessInteractable(teleporter.gameObject, true);
                }
            }

            // Pickups (item drops, command essence, etc.)
            if(ModConfig.EnableCategoryGenericPickupController.Value)
            {
                foreach(var pickup in InstanceTracker.GetInstancesList<GenericPickupController>())
                {
                    if(pickup)
                        ProcessInteractable(pickup.gameObject, !pickup.consumed);
                }
            }

            // Unknown but just in case ig
            if(ModConfig.EnableCategoryGenericInteraction.Value)
            {
                foreach(var generic in InstanceTracker.GetInstancesList<GenericInteraction>())
                {
                    if(generic)
                        ProcessInteractable(generic.gameObject, generic.interactability == Interactability.Available);
                }
            }

            // Chests, shrines, braziers, printers, multishop terminals, etc. (almost everything)
            if(ModConfig.EnableCategoryPurchaseInteraction.Value)
            {
                foreach(var purchase in InstanceTracker.GetInstancesList<PurchaseInteraction>())
                {
                    if(purchase)
                        ProcessInteractable(purchase.gameObject, purchase.available);
                }
            }

            // Barrels
            if(ModConfig.EnableCategoryBarrelInteraction.Value)
            {
                foreach(var barrel in InstanceTracker.GetInstancesList<BarrelInteraction>())
                {
                    if(barrel)
                        ProcessInteractable(barrel.gameObject, !barrel.Networkopened);
                }
            }

            // Drone combiners
            if(ModConfig.EnableCategoryDroneCombinerController.Value)
            {
                foreach(var combiner in InstanceTracker.GetInstancesList<DroneCombinerController>())
                {
                    if(combiner)
                        ProcessInteractable(combiner.gameObject, true);
                }
            }

            // Scrappers
            if(ModConfig.EnableCategoryScrapperController.Value)
            {
                foreach(var scrapper in InstanceTracker.GetInstancesList<ScrapperController>())
                {
                    if(scrapper)
                        ProcessInteractable(scrapper.gameObject, true);
                }
            }
        }


        public void ProcessInteractable(GameObject obj, bool isAvailable)
        {
            if(!isAvailable)
                return;

            var worldPos = obj.transform.position;

            var isLookingAt = Vector3.Dot(camera.transform.forward, (worldPos - camera.transform.position).normalized) > ModConfig.LookAtFloat.Value;

            if(!DisplayNameCache.TryGetValue(obj, out string label))
            {
                var displayNameProvider = obj.GetComponent<IDisplayNameProvider>();
                var displayName = displayNameProvider?.GetDisplayName() ?? "N/A";
                label = displayName;
                DisplayNameCache[obj] = label;
            }

            if(label == "N/A") 
                return;

            // filtering to do specific stuff for specific things (e.g. chest icons bigger than drones) would happen here, since we now have a displayname which is consistent for all items of that type

            var iconSprite = PingIndicator.GetInteractableIcon(obj);
            Texture2D iconTex = null;

            if(iconSprite != null && !SpriteCache.TryGetValue(iconSprite, out iconTex))
            {
                iconTex = iconSprite.texture;
                SpriteCache[iconSprite] = iconTex;
            }

            var color = obj.TryGetComponent<Highlight>(out var highlight) ? highlight.GetColor() : Color.white;

            var screenPos = camera.WorldToScreenPoint(worldPos);

            if(screenPos.z > 0)
            {
                var multiplier = ModConfig.SizeMultiplier.Value;

                var centerY = Screen.height - screenPos.y;

                var iconSize = 16f * multiplier;
                var labelHeight = 24f * multiplier;
                var padding = 4f * multiplier;

                var iconX = screenPos.x - iconSize - padding;
                var iconY = centerY - (iconSize / 2f) - (2f  * multiplier);

                var labelX = screenPos.x;
                var labelY = centerY - (labelHeight / 2f);

                var text = (ModConfig.EnableLookAtRequired.Value ? (isLookingAt ? label : "") : label);

                GUI.color = color;
                GUI.skin.label.fontSize = Mathf.RoundToInt(14f * multiplier);
                GUI.skin.label.wordWrap = false;

                if(iconTex != null)
                {
                    GUI.DrawTexture(new Rect(iconX, iconY, iconSize, iconSize), iconTex);
                }
                else
                {
                    GUI.Label(new Rect(iconX, labelY, 400, labelHeight), "◆");
                }

                if(ModConfig.EnableDropShadow.Value)
                {
                    GUI.color = Color.black;
                    GUI.Label(new Rect(labelX + 1, labelY + 1, 400, labelHeight), text);
                }

                GUI.color = color;
                GUI.Label(new Rect(labelX, labelY, 400, labelHeight), text);
            }
        }
    }
}
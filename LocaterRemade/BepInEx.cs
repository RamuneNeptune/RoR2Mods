

namespace Ramune.LocaterRemade
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class LocaterRemade : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.LocaterRemade";
        public const string PluginVersion = "1.0.0";

        public Camera camera;
        public readonly Dictionary<GameObject, ObjectConfigCacheData> ObjectConfigCache = [];
        public readonly Dictionary<GameObject, Highlight> HighlightCache = [];
        public readonly Dictionary<Sprite, Texture2D> SpriteCache = [];
        public static bool isBazaar = false;

        public class ObjectConfigCacheData
        {
            public string Label;
            public ModConfig.TrackedInstanceConfig Entry;
            public ModConfig.TrackedInstanceConfig CategoryEntry;
        }


        public void Awake()
        {
            Log.Init(Logger);
            ModConfig.Init(Config);

            On.RoR2.PickupPickerController.OnEnable += (orig, self) =>
            {
                orig(self);

                if(self.GetComponentInParent<ScrapperController>() is ScrapperController scrapper)
                    InstanceTracker.Add(scrapper);
            };

            On.RoR2.PickupPickerController.OnDisable += (orig, self) =>
            {
                if(self.GetComponentInParent<ScrapperController>() is ScrapperController scrapper)
                    InstanceTracker.Remove(scrapper);

                orig(self);
            };

            Stage.onStageStartGlobal += (stage) =>
            {
                ObjectConfigCache.Clear();
                HighlightCache.Clear();
                SpriteCache.Clear();

                if(!ModConfig.EnableBazaarCheck.Value)
                    return;

                var sceneDef = stage.sceneDef;

                if(!sceneDef)
                    return;

                var stageName = sceneDef.cachedName;

                if(string.IsNullOrEmpty(stageName))
                    return;
                
                isBazaar = stageName.ToLower().StartsWith("bazaar");
            };
        }


        public void ProcessCategory<T>(bool enabled, List<T> instances, Func<T, bool>? condition = null) where T : MonoBehaviour
        {
            if(!enabled)
                return;

            foreach(var instance in instances)
            {
                if(!instance || (condition != null && !condition(instance)))
                    continue;

                Process(instance.gameObject);
            }
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

            ProcessCategory(ModConfig.EnableCategoryGenericInteraction.Value, InstanceTracker.GetInstancesList<GenericInteraction>(), generic => generic.interactability == Interactability.Available);

            if(ModConfig.EnableBazaarCheck.Value && isBazaar)
                return;

            ProcessCategory(ModConfig.EnableCategoryTeleporterInteraction.Value, InstanceTracker.GetInstancesList<TeleporterInteraction>());

            ProcessCategory(ModConfig.EnableCategoryGenericPickupController.Value, InstanceTracker.GetInstancesList<GenericPickupController>(), pickup => !pickup.consumed);

            ProcessCategory(ModConfig.EnableCategoryPickupPickerController.Value, InstanceTracker.GetInstancesList<PickupPickerController>(), pickup => pickup.available);

            ProcessCategory(ModConfig.EnableCategoryPurchaseInteraction.Value, InstanceTracker.GetInstancesList<PurchaseInteraction>(), purchase => purchase.available);

            ProcessCategory(ModConfig.EnableCategoryBarrelInteraction.Value, InstanceTracker.GetInstancesList<BarrelInteraction>(), barrel => !barrel.Networkopened);

            ProcessCategory(ModConfig.EnableCategoryDroneCombinerController.Value, InstanceTracker.GetInstancesList<DroneCombinerController>());

            ProcessCategory(ModConfig.EnableCategoryScrapperController.Value, InstanceTracker.GetInstancesList<ScrapperController>());
        }


        public ObjectConfigCacheData GetConfigCacheData(GameObject gameObject)
        {
            if(!ObjectConfigCache.TryGetValue(gameObject, out var configCacheData))
            {
                configCacheData = new ObjectConfigCacheData();

                var displayNameProvider = gameObject.GetComponent<IDisplayNameProvider>();
                var displayName = displayNameProvider?.GetDisplayName() ?? "N/A";
                displayName = Regex.Replace(displayName, "<.*?>", "").Trim();

                configCacheData.Label = displayName;

                if(displayName != "N/A")
                {
                    configCacheData.Entry = ModConfig.TrackedInstanceConfigs.GetValueOrDefault(displayName);

                    if(ModConfig.TrackedInstanceCategoryLookup.TryGetValue(displayName, out var categoryName))
                    {
                        configCacheData.CategoryEntry = ModConfig.TrackedInstanceCategoryConfigs.GetValueOrDefault(categoryName);
                    }
                }

                ObjectConfigCache[gameObject] = configCacheData;
            }

            return configCacheData;
        }


        public void Process(GameObject obj)
        {
            var worldPos = obj.transform.position;
            var camPos = camera.transform.position;

            if(ModConfig.EnableMaxDistance.Value)
            {
                var maxDistance = ModConfig.MaxDistance.Value;
                var maxDistanceSqr = maxDistance * maxDistance;

                if((worldPos - camPos).sqrMagnitude > maxDistanceSqr)
                    return;
            }

            var screenPos = camera.WorldToScreenPoint(worldPos);

            if(screenPos.z <= 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
                return;

            var config = GetConfigCacheData(obj);

            if(config.Label == "N/A")
                return;

            var isEnabled = (config.CategoryEntry?.Enabled.Value ?? true) && (config.Entry?.Enabled.Value ?? true);

            if(!isEnabled)
                return;

            var label = config.Entry != null ? config.Entry.Label.Value : config.Label;

            var iconSprite = PingIndicator.GetInteractableIcon(obj);
            Texture2D iconTex = null;

            if(iconSprite != null && !SpriteCache.TryGetValue(iconSprite, out iconTex))
            {
                iconTex = iconSprite.texture;
                SpriteCache[iconSprite] = iconTex;
            }

            var color = config.Entry?.ColorEnabled.Value == true ? config.Entry.Color.Value : config.CategoryEntry?.ColorEnabled.Value == true ? config.CategoryEntry.Color.Value : GetFallbackColor(obj);

            Color GetFallbackColor(GameObject gameObject)
            {
                if(!HighlightCache.TryGetValue(gameObject, out var highlight))
                {
                    gameObject.TryGetComponent(out highlight);
                    HighlightCache[gameObject] = highlight;
                }

                return highlight ? highlight.GetColor() : Color.white;
            }

            var globalScaleMultiplier = ModConfig.GlobalScale.Value;

            var globalIconScaleMultiplier = ModConfig.GlobalIconScale.Value * globalScaleMultiplier;
            var globalLabelScaleMultiplier = ModConfig.GlobalLabelScale.Value * globalScaleMultiplier;

            var iconScaleMultiplier = (config.CategoryEntry?.IconScale.Value ?? 1f) * (config.Entry?.IconScale.Value ?? 1f);
            var labelScaleMultiplier = (config.CategoryEntry?.LabelScale.Value ?? 1f) * (config.Entry?.LabelScale.Value ?? 1f);

            var centerY = Screen.height - screenPos.y;

            var iconSize = 16f * globalIconScaleMultiplier * iconScaleMultiplier;
            var labelHeight = 24f * globalLabelScaleMultiplier * labelScaleMultiplier;
            var fontSize = Mathf.RoundToInt(14f * globalLabelScaleMultiplier * labelScaleMultiplier);
            var padding = 4f * globalScaleMultiplier;

            var iconX = screenPos.x - iconSize - padding;
            var iconY = centerY - (iconSize / 2f);

            var labelX = screenPos.x;
            var labelY = centerY - (labelHeight / 2f);

            GUI.skin.label.fontSize = fontSize;
            GUI.skin.label.wordWrap = false;
            GUI.color = color;

            if(iconTex != null)
            {
                GUI.DrawTexture(new Rect(iconX, iconY, iconSize, iconSize), iconTex);
            }
            else
            {
                GUI.Label(new Rect(iconX, iconY, iconSize, iconSize), "◆");
            }

            // If EnableLookAtRequired && !isLookingAt
            if(ModConfig.EnableLookAtRequired.Value && !(Vector3.Dot(camera.transform.forward, (worldPos - camPos).normalized) > ModConfig.LookAtFloat.Value))
                return;

            if(ModConfig.EnableDropShadow.Value)
            {
                GUI.color = Color.black;
                GUI.Label(new Rect(labelX + 1, labelY + 1, 400 * labelScaleMultiplier, labelHeight), label);
            }

            GUI.color = color;
            GUI.Label(new Rect(labelX, labelY, 400 * labelScaleMultiplier, labelHeight), label);
        }
    }
}
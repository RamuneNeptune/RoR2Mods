

namespace Ramune.RememberRunSettings
{
    //[BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class RememberRunSettings : BaseUnityPlugin
    {
        public const string PluginGUID = PluginName;
        public const string PluginAuthor = "RamuneNeptune";
        public const string PluginName = "Ramune.RememberRunSettings";
        public const string PluginVersion = "1.0.0";

        public string RunSettingsFolder { get; private set; }
        public string GetRunSettingsPath(LocalUser localUser) => Path.Combine(RunSettingsFolder, localUser.userProfile.fileName + ".json");
        public string GetTempRunSettingsPath(LocalUser localUser) => GetRunSettingsPath(localUser) + ".tmp";

        public static readonly string[] InvalidRuleNamePrefixes = ["Items.", "Equipment.", "Drones."];
        public Dictionary<string, int> SavedVotes = [];
        public Coroutine DelayedSaveCoroutine;
        public LocalUser CurrentLocalUser;


        public void Awake()
        {
            Log.Init(Logger);

            RunSettingsFolder = Path.Combine(Paths.ConfigPath, "Ramune.RememberRunSettings");
            Directory.CreateDirectory(RunSettingsFolder);

            LocalUserManager.onUserSignIn += (self) =>
            {
                if(self == null || self.userProfile == null)
                    return;

                CurrentLocalUser = self;
                LoadSettings(self);
            };

            On.RoR2.PreGameRuleVoteController.SetVote += (orig, self, index, value) =>
            {
                orig(self, index, value);

                if(!RoR2Application.isInSinglePlayer && !NetworkServer.active || CurrentLocalUser == null || CurrentLocalUser.userProfile == null)   
                    return;

                var ruleDef = RuleCatalog.GetRuleDef(index);

                if(!IsValidRule(ruleDef))
                    return;
                
                var key = ruleDef.globalName;

                if(SavedVotes.TryGetValue(key, out var old) && old == value)
                    return;
                    
                SavedVotes[key] = value;
                SaveSettings(CurrentLocalUser);
            };

            On.RoR2.PreGameRuleVoteController.Start += (orig, self) =>
            {
                orig(self);

                if(!RoR2Application.isInSinglePlayer && !NetworkServer.active || CurrentLocalUser == null || CurrentLocalUser.userProfile == null || SavedVotes.Count <= 0 || self.votes == null)
                    return;

                var requiresSave = false;
                var keysToRemove = new List<string>();

                foreach(var kvp in SavedVotes)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;

                    var ruleDef = RuleCatalog.FindRuleDef(key);

                    if(ruleDef == null)
                    {
                        keysToRemove.Add(key);
                        requiresSave = true;
                        continue;
                    }

                    var index = ruleDef.globalIndex;

                    if(IsValidRule(ruleDef) && index >= 0 && index < self.votes.Length && value < ruleDef.choices.Count && self.votes[index].choiceValue != value)
                    {
                        self.votes[index].choiceValue = value;
                        self.SetDirtyBit(2U);
                        PreGameRuleVoteController.shouldUpdateGameVotes = true;
                    }
                }

                if(requiresSave)
                {
                    foreach(var key in keysToRemove)
                        SavedVotes.Remove(key);

                    SaveSettings(CurrentLocalUser);
                }
            };
        }


        public static bool IsValidRule(RuleDef ruleDef)
        {
            if(ruleDef == null || string.IsNullOrEmpty(ruleDef.globalName)) 
                return false;

            for(int i = 0; i < InvalidRuleNamePrefixes.Length; i++)
            {
                if(ruleDef.globalName.StartsWith(InvalidRuleNamePrefixes[i], StringComparison.Ordinal))
                    return false;
            }

            return true;
        }


        public void LoadSettings(LocalUser localUser)
        {
            SavedVotes = [];

            var settingsPath = GetRunSettingsPath(localUser);

            if(!File.Exists(settingsPath))
                return;

            try
            {
                string json = File.ReadAllText(settingsPath);
                SavedVotes = JsonConvert.DeserializeObject<Dictionary<string, int>>(json) ?? [];
            }
            catch(Exception ex)
            {
                Log.Error($"Failed to load {Path.GetFileName(settingsPath)}: {ex.Message}");
                SavedVotes = [];
            }
        }


        public void SaveSettings(LocalUser localUser)
        {
            if(DelayedSaveCoroutine != null)
            {
                StopCoroutine(DelayedSaveCoroutine);
            }

            DelayedSaveCoroutine = StartCoroutine(DelayedSave(localUser));
        }


        public IEnumerator DelayedSave(LocalUser localUser)
        {
            yield return new WaitForSecondsRealtime(1f);

            var settingsPath = GetRunSettingsPath(localUser);
            var settingsTempPath = GetTempRunSettingsPath(localUser);

            try
            {
                File.WriteAllText(settingsTempPath, JsonConvert.SerializeObject(SavedVotes, Formatting.None));

                if(File.Exists(settingsPath))
                {
                    File.Replace(settingsTempPath, settingsPath, null);
                }
                else
                {
                    File.Move(settingsTempPath, settingsPath);
                }
            }
            catch(Exception ex)
            {
                Log.Error($"Failed to save {Path.GetFileName(settingsPath)}: {ex.Message}");
            }
            finally
            {
                try
                {
                    if(File.Exists(settingsTempPath))
                    {
                        File.Delete(settingsTempPath);
                    }
                }
                catch(Exception cleanupEx)
                {
                    Log.Error($"Failed to cleanup temp file {settingsTempPath}: {cleanupEx.Message}");
                }

                DelayedSaveCoroutine = null;
            }
        }
    }
}
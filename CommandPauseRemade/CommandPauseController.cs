

namespace Ramune.CommandPauseRemade
{
    public class CommandPauseController : MonoBehaviour
    {
        public static readonly BuffDef[] PauseBuffDefs = [RoR2Content.Buffs.HealingDisabled, RoR2Content.Buffs.Immune, DLC3Content.Buffs.Untargetable, RoR2Content.Buffs.Entangle];

        // Track the amount of times each master has opened a Command menu, used to deal with the mess of a player opening several menus within the protection delay time
        public static readonly Dictionary<CharacterMaster, int> TrackedMasterInstances = [];

        public NetworkUIPromptController networkUIPromptController;

        public CharacterMaster trackedMaster;


        public void Update()
        {
            var currentMaster = networkUIPromptController ? networkUIPromptController.currentParticipantMaster : null;

            if(currentMaster == trackedMaster)
                return;

            if(trackedMaster)
            {
                trackedMaster.GetBody()?.RemoveBuff(RoR2Content.Buffs.Entangle);

                var currentInstanceForMaster = TrackedMasterInstances.GetValueOrDefault(trackedMaster);

                // If protection delay is > 0, start a coroutine that waits until after the delay to remove protections
                if(ModConfig.ProtectionDelay.Value > 0f)
                {
                    RoR2Application.instance.StartCoroutine(StartDelay(trackedMaster, currentInstanceForMaster));
                }
                // If protection delay is set to 0, skip starting a coroutine and just immediately remove protections
                else DisableProtection(trackedMaster, currentInstanceForMaster);
            }

            trackedMaster = currentMaster;

            if(trackedMaster)
            {
                // New menu instance opened
                // +1 to the instance count for this trackedMaster
                TrackedMasterInstances[trackedMaster] = (TrackedMasterInstances.GetValueOrDefault(trackedMaster)) + 1;

                ToggleProtection(trackedMaster, true);
            }
        }

        // Re-run Update() logic instead of re-using code here, and the Update() logic is specifically able to safely run without issues of null references
        public void OnDestroy() => Update();


        public static void ToggleProtection(CharacterMaster master, bool enabled)
        {
            if(!master)
                return;

            var body = master.GetBody();

            if(!body) 
                return;

            body.ToggleBuffBulk(enabled, PauseBuffDefs);
        }


        public static void DisableProtection(CharacterMaster master, int currentInstanceForMaster)
        {
            // If currentInstanceForMaster matches the total instance count for this master, the player hasn't opened another menu within the protection delay time (all clear to remove protections)
            // If they have opened another menu within the protection delay time, this does nothing and waits for them to close the new menu before re-running this logic
            if(master && TrackedMasterInstances.GetValueOrDefault(master) == currentInstanceForMaster)
                ToggleProtection(master, false);
        }


        public static IEnumerator StartDelay(CharacterMaster master, int currentInstanceForMaster)
        {
            yield return new WaitForSeconds(ModConfig.ProtectionDelay.Value);
            DisableProtection(master, currentInstanceForMaster);
        }
    }
}


namespace Ramune.CommandPauseRemade
{
    public class CommandPauseController : MonoBehaviour
    {
        public static readonly BuffDef[] PauseBuffDefs = [RoR2Content.Buffs.HealingDisabled, RoR2Content.Buffs.Immune, DLC3Content.Buffs.Untargetable, RoR2Content.Buffs.Entangle];

        // The amount of times a given master has opened a PickupPickerController menu
        public static readonly Dictionary<CharacterMaster, int> MasterMenuOpenedCount = [];

        public NetworkUIPromptController networkUIPromptController;

        public CharacterMaster master;


        public void OnDestroy() => Update();


        public void Update()
        {
            var currentMaster = networkUIPromptController ? networkUIPromptController.currentParticipantMaster : null;

            if(currentMaster == master)
                return;

            if(master && master.hasBody)
            {
                // Remove Entangle immediately, instead of forcing the player to be immovable until the protection is up
                master.GetBody().RemoveBuff(RoR2Content.Buffs.Entangle);    

                var currentMasterMenuOpenedCount = MasterMenuOpenedCount.GetValueOrDefault(master);

                // If protection delay is > 0, start a coroutine that waits until after the delay to remove protections
                if(ModConfig.ProtectionDelay.Value > 0f)
                {
                    RoR2Application.instance.StartCoroutine(StartDelay(master, currentMasterMenuOpenedCount));
                }
                // If protection delay is set to 0, skip starting a coroutine and just immediately remove protections
                else DisableProtection(master, currentMasterMenuOpenedCount);
            }

            master = currentMaster;

            if(master)
            {
                // +1 to menu opened count for this master
                MasterMenuOpenedCount[master] = (MasterMenuOpenedCount.GetValueOrDefault(master)) + 1;

                ToggleProtection(master, true);
            }
        }


        public static void ToggleProtection(CharacterMaster master, bool enabled)
        {
            if(!master || !master.hasBody)
                return;

            master.GetBody().ToggleBuffBulk(enabled, PauseBuffDefs);
        }


        public static void DisableProtection(CharacterMaster master, int currentMasterMenuOpenedCount)
        {
            if(!master || MasterMenuOpenedCount.GetValueOrDefault(master) != currentMasterMenuOpenedCount)
                return;
            
            ToggleProtection(master, false);
        }


        public static IEnumerator StartDelay(CharacterMaster master, int currentMasterMenuOpenedCount)
        {
            yield return new WaitForSeconds(ModConfig.ProtectionDelay.Value);

            DisableProtection(master, currentMasterMenuOpenedCount);
        }
    }
}


namespace Ramune.CommandPauseRemade
{
    public class CommandPauseController : MonoBehaviour
    {
        public static readonly BuffDef[] PauseBuffDefs = [RoR2Content.Buffs.HealingDisabled, RoR2Content.Buffs.Immune, DLC3Content.Buffs.Untargetable, RoR2Content.Buffs.Entangle];

        public static readonly Dictionary<CharacterMaster, int> MasterSessionIds = [];

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

                if(ModConfig.ProtectionDelay.Value > 0f)
                {
                    RoR2Application.instance.StartCoroutine(StartDelay(trackedMaster, (MasterSessionIds.GetValueOrDefault(trackedMaster))));
                }
                else DisableProtection(trackedMaster, (MasterSessionIds.GetValueOrDefault(trackedMaster)));
            }

            trackedMaster = currentMaster;

            if(trackedMaster)
            {
                MasterSessionIds[trackedMaster] = (MasterSessionIds.GetValueOrDefault(trackedMaster)) + 1;

                ToggleProtection(trackedMaster, true);
            }
        }


        public void OnDestroy() => Update();


        public static void ToggleProtection(CharacterMaster master, bool enabled)
        {
            if(!master)
                return;

            master.godMode = enabled;
            master.GetBody()?.ToggleBuffBulk(enabled, PauseBuffDefs);
        }


        public static void DisableProtection(CharacterMaster master, int ticket)
        {
            if(master && MasterSessionIds.GetValueOrDefault(master) == ticket)
                ToggleProtection(master, false);
        }


        public static IEnumerator StartDelay(CharacterMaster master, int ticket)
        {
            yield return new WaitForSeconds(ModConfig.ProtectionDelay.Value);
            DisableProtection(master, ticket);
        }
    }
}
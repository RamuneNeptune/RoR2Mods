

namespace Ramune.CommandPauseRemade
{
    public class CommandPauseController : MonoBehaviour
    {
        public static readonly BuffDef[] PauseBuffDefs = [RoR2Content.Buffs.HealingDisabled, RoR2Content.Buffs.Immune, DLC3Content.Buffs.Untargetable, RoR2Content.Buffs.Entangle];

        public static readonly Dictionary<CharacterMaster, int> masterSessionIds = [];

        public static readonly WaitForSeconds DisableDelay = new(0.225f);

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

                RoR2Application.instance.StartCoroutine(DisableProtection(trackedMaster, (masterSessionIds.GetValueOrDefault(trackedMaster))));
            }

            trackedMaster = currentMaster;

            if(trackedMaster)
            {
                masterSessionIds[trackedMaster] = (masterSessionIds.GetValueOrDefault(trackedMaster)) + 1;

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


        public static IEnumerator DisableProtection(CharacterMaster master, int ticket)
        {
            yield return DisableDelay;

            if(master && masterSessionIds.GetValueOrDefault(master) == ticket)
                ToggleProtection(master, false);
        }
    }
}


namespace Ramune.CommandPauseRemade
{
    public static class Extensions
    {
        public static void ToggleBuffBulk(this CharacterBody body, bool add, params BuffDef[] buffDefs)
        {
            if(!body || !NetworkServer.active)
                return;

            foreach(var buff in buffDefs)
            {
                if(add)
                {
                    body.AddBuff(buff);
                }
                else
                {
                    while(body.HasBuff(buff))
                    {
                        body.RemoveBuff(buff);
                    }
                }
            }

            return;
        }
    }
}
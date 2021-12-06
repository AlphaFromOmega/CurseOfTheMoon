using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CurseOfTheMoon.Content.Buffs
{
    public class Bleeding : GlobalBuff
    {
        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            base.Update(type, npc, ref buffIndex);
            if (type == BuffID.Bleeding)
            {
                npc.lifeRegen -= 12;
            }
        }
    }
}

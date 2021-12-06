using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CurseOfTheMoon.Content.NPCs.Boss.FrigaroBoss
{
	[AutoloadBossHead]
   	public class FrigaroCharger : ModNPC
	{
		public override string Texture => "CurseOfTheMoon/Content/NPCs/Boss/Frigaro";
		public override string BossHeadTexture => "CurseOfTheMoon/Content/NPCs/Boss/Frigaro_Head_Boss";
		public int owner;

		private float rallyDistance = 0;
		private const float rallyDistanceMax = 1800;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Frigaro");
			Main.npcFrameCount[Type] = 7;

			// Specify the debuffs it is immune to
			NPCDebuffImmunityData debuffData = new()
            {
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned,

					BuffID.Confused // Most NPCs have this
				}
			};
			NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
		}
		public override void SetDefaults()
		{
			DrawOffsetY = 32;
			NPC.width = 110;
			NPC.height = 110;
			NPC.damage = 40;
			NPC.defense = 12;
			NPC.lifeMax = 5000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.SpawnWithHigherTime(30);
			NPC.npcSlots = 1f;

			NPC.aiStyle = -1;
		}

        public override void AI()
		{
			NPC.velocity.X = 16 - 32 * NPC.direction;
			if (rallyDistance > rallyDistanceMax)
            {
				NPC.EncourageDespawn(10);
			}
			else
            {
				rallyDistance += NPC.velocity.X;
            }
			if (Main.npc[owner] != null)
			{
				NPC.life = Main.npc[owner].life;
			}
			else
            {
				NPC.EncourageDespawn(10);
			}
			NPC.spriteDirection = NPC.direction;
		}
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
			Main.npc[owner].StrikeNPCNoInteraction(damage, knockback, 0, crit);
			if (Main.npc[owner].life <= 0)
			{
				NPC.immortal = true;
			}
			base.OnHitByProjectile(projectile, damage, knockback, crit);
		}
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
		{
			Main.npc[owner].StrikeNPCNoInteraction(damage, knockback, 0, crit);
			if (Main.npc[owner].life <= 0)
			{
				NPC.immortal = true;
			}
			base.OnHitByItem(player, item, damage, knockback, crit);
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = 4 * frameHeight;
		}
	}
}

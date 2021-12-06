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
using CurseOfTheMoon.Content.NPCs.Boss.FrigaroBoss;
using CurseOfTheMoon.Content.Projectiles.Bosses.Frigaro;
using Terraria.GameContent.ItemDropRules;

namespace CurseOfTheMoon.Content.NPCs.Boss
{
	[AutoloadBossHead]
	public class Frigaro : ModNPC
	{
		private float chargeTarget;
		private int nextAttack = Main.rand.Next(300, 600);

		private const int AI_STATE = 0;
		private const int STATE_FLEE = -100;
		private const int STATE_CHARGE = -1;
		private const int STATE_RALLY = -2;
		private const int STATE_FOLLOW = 0;
		private const int STATE_CHARGE_ALERT = 1;
		private const int STATE_RALLY_ALERT = 2;
		private const int STATE_SPIT = 3;

		private const int AI_TIMER = 1;
		public float AIState
		{
			get => NPC.ai[AI_STATE];
			set => NPC.ai[AI_STATE] = value;
		}
		public float AITimer
		{
			get => NPC.ai[AI_TIMER];
			set => NPC.ai[AI_TIMER] = value;
		}
		public static int ChargeWait()
		{
			int wait = 40;

			if (Main.expertMode)
			{
				wait /= 2;
			}

			if (Main.getGoodWorld)
			{
				wait /= 4;
			}

			return wait;
		}
		public static int RallyInterval()
		{
			int inv = 40;

			if (Main.expertMode)
			{
				inv -= 10;
			}

			if (Main.getGoodWorld)
			{
				inv -= 10;
			}

			return inv;
		}
		private int frame;
		private int frameTime = 0;
		private const int FRAME_SPEED = 3;
		private readonly int[] ANIM_IDLE = {0, 1, 2};
		private readonly int[] ANIM_OPEN = {3, 4};
		private readonly int[] ANIM_CLOSE = {5, 6};
		private readonly int[] ANIM_CHOMP = { 3, 4, 5, 6 };

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 7;

			// Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			// Specify the debuffs it is immune to
			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
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
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 5);
			NPC.SpawnWithHigherTime(256);
			NPC.boss = true;
			NPC.npcSlots = 10f; 

			NPC.aiStyle = -1;

			// Custom boss bar
			//NPC.BossBar = ModContent.GetInstance<MinionBossBossBar>();

			// Important if this boss has a treasure bag
			//BossBag = ModContent.ItemType<MinionBossBag>();

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				Music = MusicID.Boss1;
				//Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ropocalypse2");
			}
		}
		public override bool CheckActive() 
		{
			if (AIState == STATE_FLEE)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("The head of a doctor who was chocked by one of their own patients.")
			});
		}

        public override void AI()
		{
			Player player = Main.player[NPC.target];
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest(true);
			}
			switch (AIState)
            {
				case STATE_FLEE:
					NPC.velocity.Y -= 0.04f;
					Mod.Logger.Debug($"{Name} [{NPC.whoAmI}] is fleeing.");
					NPC.EncourageDespawn(10);
					break;
				case STATE_FOLLOW:
					ChangeFrame(ANIM_IDLE);
					player = Main.player[NPC.target];
					NPC.direction = player.Center.X < NPC.Center.X ? 1 : 0;
					Vector2 sidePosition = Vector2.Subtract(new(player.Center.X - 512 + NPC.direction * 1024, player.Center.Y), new Vector2(NPC.width/2, NPC.height/2));
					if (sidePosition.Distance(NPC.position) >= 8)
					{
						TargetPoint(sidePosition, 8);
					}
					else
					{
						TargetPoint(sidePosition + new Vector2(Main.rand.Next(4) - 2, Main.rand.Next(4) - 2), 1);
						if (AITimer > nextAttack)
						{
							AIState = Main.rand.Next(3) + 1;
							Mod.Logger.Debug($"Frigaro State - {AIState}");
							SoundEngine.PlaySound(SoundID.Roar, NPC.Center, 0);
							NPC.velocity = Vector2.Zero;
							AITimer = 0;
							nextAttack = Main.rand.Next(60, 300);
						}
						else
						{
							AITimer++;
						}
					}
					break;
				case STATE_CHARGE_ALERT:
					ChangeFrame(ANIM_OPEN,true);
					if (AITimer > ChargeWait())
					{
						chargeTarget = player.Center.X - 512 + -(NPC.direction - 1) * 1024;
						AIState = STATE_CHARGE;
					}
					else
					{
						AITimer++;
					}
					break;
				case STATE_CHARGE:
					NPC.velocity.X = 16 - 32 * NPC.direction;
					if (Math.Abs(NPC.position.X - chargeTarget) <= 32)
					{
						NPC.velocity.X = 0;
						ChangeFrame(ANIM_CLOSE, true);
						if (frame == 6)
						{
							AIState = STATE_FOLLOW;
						}
					}
					break;
				case STATE_RALLY_ALERT:
					ChangeFrame(ANIM_OPEN, true);
					NPC.velocity.X = 32 * NPC.direction - 16;
					NPC.spriteDirection = NPC.direction - 1;
					if (Math.Abs(NPC.position.X - (player.position.X - (2048 - NPC.direction * 5096))) <= 32)
					{
						NPC.velocity.X = 0;
						AIState = STATE_RALLY;
					}
					break;
				case STATE_RALLY:
					TargetPoint(new Vector2(player.position.X - (2048 - NPC.direction * 5096), player.Center.Y + Main.rand.Next(640) - 320), 128);
					if (AITimer >= 600)
					{
						SoundEngine.PlaySound(SoundID.Roar, NPC.position, 0);
						AIState = STATE_CHARGE;
						NPC.velocity = Vector2.Zero;
						chargeTarget = Math.Abs(player.position.X - (512 - NPC.direction * 1024));
						AITimer = 0;
						break;
					}
					else if (AITimer % RallyInterval() == 0)
					{
						int index = NPC.NewNPC((int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FrigaroCharger>());
						NPC spawned = Main.npc[index];
						if (spawned.ModNPC is FrigaroCharger charger)
						{
							charger.owner = NPC.whoAmI;
							spawned.life = NPC.life;
							spawned.direction = NPC.direction;
						}
					}
					AITimer++;
					break;
				case STATE_SPIT:
					if (AITimer % 20 > 19 - 4)
					{
						ChangeFrame(ANIM_CHOMP, false, 1);
					}
					if (AITimer % 20 == 19)
					{
						ChangeFrame(new int[] { 6 });
						for (int i = 0; i < Main.rand.Next(1, 4); i++)
                        {
							Projectile.NewProjectile(NPC.GetProjectileSpawnSource(), NPC.Center - ((NPC.direction * 2 - 1) * NPC.width / 2) * Vector2.UnitX, (Vector2.UnitX * -(NPC.direction * 2 - 1) * Main.rand.NextFloat(4, 12)).RotatedBy(MathHelper.ToRadians(40 * (NPC.direction * 2 - 1))).RotatedByRandom(MathHelper.ToRadians(20)), ModContent.ProjectileType<Spit>(), NPC.damage / 2, 0f, Main.myPlayer);
						}
						if (AITimer >= 120)
						{
							AIState = STATE_FOLLOW;
						}
					}
					AITimer++;
					break;
			}
			if (player.dead)
			{
				NPC.TargetClosest(true);
				if (player.dead)
				{
					AIState = STATE_FLEE;
				}
			}
		}
        public override void FindFrame(int frameHeight)
		{
			NPC.spriteDirection = NPC.direction;
			NPC.frame.Y = frame * frameHeight;
		}
		private void ChangeFrame(int[] frames, bool freeze = false, int fspeed = FRAME_SPEED)
        {
			if (frameTime >= fspeed)
            {
				bool checkFrame = false;
				frameTime = 0;
				for (int i = 0; i < frames.Length; i++)
                {
					if (frames[i] == frame)
                    {
						checkFrame = true;
						if (i + 1 >= frames.Length)
                        {
							if (!freeze)
							{
								frame = frames[0];
							}
                        }
						else
						{
							frame = frames[i + 1];
						}
						break;
					}
                }
				if (!checkFrame)
                {
					frame = frames[0];
				}
			}
			else
            {
				frameTime++;
			}
        }

		public override bool? CanBeHitByItem(Player player, Item item)
        {
			if (AIState == STATE_RALLY)
			{
				return false;
			}
			return base.CanBeHitByItem(player, item);
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (AIState == STATE_RALLY)
			{
				return false;
			}
			return base.CanBeHitByProjectile(projectile);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (AIState == STATE_RALLY)
            {
				return false;
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        private bool TargetPoint(Vector2 point, float speed)
        {
			Vector2 direction = NPC.position.DirectionTo(point);
			if (point.Distance(NPC.position) > speed)
			{
				NPC.velocity = direction * speed;
				return false;
			}
			else
            {
				NPC.velocity = Vector2.Zero;
				NPC.position = point;
				return true;
			}
        }
	}
}

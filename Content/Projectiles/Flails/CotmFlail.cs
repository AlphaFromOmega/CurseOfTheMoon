using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using System.Collections.Generic;

namespace CurseOfTheMoon.Content.Projectiles.Flails
{
    public abstract class CotmFlail : ModProjectile
	{
		public List<string> chainTexturePath = new();

		public int maxExtend;
		public float shotSpeed;
		public float returnDistance;
		public float returnFast;

		public virtual void ChainTexture()
		{
			chainTexturePath.Add((GetType().Namespace + "." + Name + "Chain").Replace('.', '/'));
		}
        public override void SetDefaults()
        {
            base.SetDefaults();
			ChainTexture();
		}

        private Vector2 wetVelocity = Vector2.Zero;
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
			{
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
			{
				Projectile.Kill();
				return;
			}
			Vector2 mountedCenter = player.MountedCenter;
			bool flag = true;
			bool hitCheck = false;
			float maxDistance = 800f;
			float velSpeed = 3f;
			float num6 = 6f;
			float num8 = 1f;
			float num9 = 14f;
			int num10 = 60;
			int num11 = 10;
			int rotateIFrames = 20;
			int retractIFrames = 10;
			int num14 = maxExtend + 5;
			float meleeSpeedInverted = 1f / player.meleeSpeed;
			shotSpeed *= meleeSpeedInverted;
			num8 *= meleeSpeedInverted;
			num9 *= meleeSpeedInverted;
			velSpeed *= meleeSpeedInverted;
			returnDistance *= meleeSpeedInverted;
			num6 *= meleeSpeedInverted;
			returnFast *= meleeSpeedInverted;
			float num16 = shotSpeed * maxExtend;
			float num17 = num16 + 160f;
			Projectile.localNPCHitCooldown = num11;
			switch ((int)Projectile.ai[0])
			{
				case 0: // Rotate Around Player
					{
						hitCheck = true;
						if (Projectile.owner == Main.myPlayer)
						{
							Vector2 origin = mountedCenter;
							Vector2 mouseWorld = Main.MouseWorld;
							Vector2 mouseDirection = origin.DirectionTo(mouseWorld).SafeNormalize(Vector2.UnitX * (float)player.direction);
							player.ChangeDir((mouseDirection.X > 0f) ? 1 : (-1));
							if (!player.channel)
							{
								Projectile.ai[0] = 1f;
								Projectile.ai[1] = 0f;
								Projectile.velocity = mouseDirection * shotSpeed + player.velocity;
								Projectile.Center = mountedCenter;
								Projectile.netUpdate = true;
								for (int i = 0; i < 200; i++)
								{
									Projectile.localNPCImmunity[i] = 0;
								}
								Projectile.localNPCHitCooldown = retractIFrames;
								break;
							}
						}
						Projectile.localAI[1] += 1f;
						Vector2 rotationPosition = Utils.RotatedBy(new Vector2(player.direction), Math.PI * 10f * (Projectile.localAI[1] / 60f) * (float)player.direction);
						rotationPosition.Y *= 0.8f;
						if (rotationPosition.Y * player.gravDir > 0f)
						{
							rotationPosition.Y *= 0.5f;
						}
						Projectile.Center = mountedCenter + rotationPosition * 30f;
						Projectile.velocity = Vector2.Zero;
						Projectile.localNPCHitCooldown = rotateIFrames;
						break;
					}
				case 1: // Extending Flail
					{
						bool flag3 = Projectile.ai[1]++ >= maxExtend;
						flag3 |= Projectile.Distance(mountedCenter) >= maxDistance;
						if (player.controlUseItem)
						{
							Projectile.ai[0] = 6f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
							Projectile.damage /= 2;
							break;
						}
						if (flag3)
						{
							Projectile.ai[0] = 2f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.3f;
							Projectile.damage /= 2;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						Projectile.localNPCHitCooldown = retractIFrames;
						break;
					}
				case 2: // Retract Allow Grav
					{
						Vector2 val5 = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= returnDistance)
						{
							Projectile.Kill();
							return;
						}
						if (player.controlUseItem)
						{
							Projectile.ai[0] = 6f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
						}
						else
						{
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(val5 * returnDistance, velSpeed);
							player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						}
						break;
					}
				case 3:
					{
						if (!player.controlUseItem)
						{
							Projectile.ai[0] = 4f;
							Projectile.ai[1] = 0f;
							Projectile.netUpdate = true;
							break;
						}
						float num18 = Projectile.Distance(mountedCenter);
						Projectile.tileCollide = Projectile.ai[1] == 1f;
						bool flag4 = num18 <= num16;
						if (flag4 != Projectile.tileCollide)
						{
							Projectile.tileCollide = flag4;
							Projectile.ai[1] = (Projectile.tileCollide ? 1 : 0);
							Projectile.netUpdate = true;
						}
						if (num18 > num10)
						{
							if (num18 >= num16)
							{
								Projectile.velocity *= 0.5f;
								Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * num9, num9);
							}
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * num9, num8);
						}
						else
						{
							if (Projectile.velocity.Length() < 6f)
							{
								Projectile.velocity.X *= 0.96f;
								Projectile.velocity.Y += 0.2f;
							}
							if (player.velocity.X == 0f)
							{
								Projectile.velocity.X *= 0.96f;
							}
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						break;
					}
				case 4: // Retract (No Grav)
					{
						Projectile.tileCollide = false;
						Vector2 val3 = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= returnFast)
						{
							Projectile.Kill();
							return;
						}
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(val3 * returnFast, num6);
						Vector2 target = Projectile.Center + Projectile.velocity;
						Vector2 val4 = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
						if (Vector2.Dot(val3, val4) < 0f)
						{
							Projectile.Kill();
							return;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
						break;
					}
				case 5:
					if (Projectile.ai[1]++ >= num14)
					{
						Projectile.ai[0] = 6f;
						Projectile.ai[1] = 0f;
						Projectile.netUpdate = true;
					}
					else
					{
						Projectile.localNPCHitCooldown = retractIFrames;
						Projectile.velocity.Y += 0.6f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}
					break;
				case 6: // Falling
					if (!player.controlUseItem || Projectile.Distance(mountedCenter) > num17)
					{
						Projectile.ai[0] = 4f;
						Projectile.ai[1] = 0f;
						Projectile.netUpdate = true;
					}
					else
					{
						Projectile.velocity.Y += 0.8f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
					}
					break;
			}
			Projectile.direction = (Projectile.velocity.X > 0f) ? 1 : (-1);
			Projectile.spriteDirection = Projectile.direction;
			Projectile.ownerHitCheck = hitCheck;
			if (flag)
			{
				if (Projectile.velocity.Length() > 1f)
				{
					Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f;
				}
				else
				{
					Projectile.rotation += Projectile.velocity.X * 0.1f;
				}
			}
			Projectile.timeLeft = 2;
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2);
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
			if (Projectile.Center.X < mountedCenter.X)
			{
				player.itemRotation += (float)Math.PI;
			}
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			DrawChains(Projectile);
			return base.PreDraw(ref lightColor);
		}

		private void DrawChains(Projectile proj)
		{
			Vector2 playerArmPosition = Main.GetPlayerArmPosition(proj);
			Vector2 chainOffset = proj.Center;
			Vector2 arm = playerArmPosition.MoveTowards(chainOffset, 4f) - chainOffset;
			Vector2 armVector = arm.SafeNormalize(Vector2.Zero);

			var j = 0;
			Texture2D chainTexture = (Texture2D)ModContent.Request<Texture2D>(chainTexturePath[j]);
			float chainHeight = chainTexture.Height;
			float chainRotation = armVector.ToRotation() + (float)Math.PI / 2f;

			for (float i = arm.Length() + chainHeight / 2f; i > 0f; i -= chainHeight)
			{
				chainTexture = (Texture2D)ModContent.Request<Texture2D>(chainTexturePath[j]);
				Color lightColor = Lighting.GetColor((int)chainOffset.X / 16, (int)(chainOffset.Y / 16f));
				Main.spriteBatch.Draw(chainTexture, chainOffset - Main.screenPosition, null, lightColor, chainRotation, chainTexture.Size() / 2f, 1f, 0, 0f);
				chainOffset += armVector * chainHeight;
				j += chainTexturePath.Count > j + 1 ? 1 : 0;
			}
		}
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			MovementCollision(wetVelocity, oldVelocity);
			return false;
		}
		private void MovementCollision(Vector2 wetVelocity, Vector2 lastVelocity)
		{
			int num = 10;
			int num2 = 0;
			Vector2 val = Projectile.velocity;
			float num3 = 0.2f;
			if (Projectile.ai[0] == 1f || Projectile.ai[0] == 5f)
			{
				num3 = 0.4f;
			}
			if (Projectile.ai[0] == 6f)
			{
				num3 = 0f;
			}
			if (lastVelocity.X != Projectile.velocity.X)
			{
				if (Math.Abs(lastVelocity.X) > 4f)
				{
					num2 = 1;
				}
				Projectile.velocity.X = (0f - lastVelocity.X) * num3;
				Projectile.localAI[0] += 1f;
			}
			if (lastVelocity.Y != Projectile.velocity.Y)
			{
				if (Math.Abs(lastVelocity.Y) > 4f)
				{
					num2 = 1;
				}
				Projectile.velocity.Y = (0f - lastVelocity.Y) * num3;
				Projectile.localAI[0] += 1f;
			}
			if (Projectile.ai[0] == 1f)
			{
				Projectile.ai[0] = 5f;
				Projectile.localNPCHitCooldown = num;
				Projectile.netUpdate = true;
				Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
				Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
				num2 = 2;
				CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out var causedShockwaves);
				CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, val);
				Projectile.position -= val;
			}
			if (num2 > 0)
			{
				Projectile.netUpdate = true;
				for (int i = 0; i < num2; i++)
				{
					Collision.HitTiles(Projectile.position, val, Projectile.width, Projectile.height);
				}
				SoundEngine.PlaySound(0, (int)Projectile.position.X, (int)Projectile.position.Y);
			}
			if (Projectile.ai[0] != 3f && Projectile.ai[0] != 0f && Projectile.ai[0] != 5f && Projectile.ai[0] != 6f && Projectile.localAI[0] >= 10f)
			{
				Projectile.ai[0] = 4f;
				Projectile.netUpdate = true;
			}
			if (Projectile.wet)
			{
				wetVelocity = Projectile.velocity;
			}
		}

		private static void CreateImpactExplosion(int dustAmountMultiplier, Vector2 explosionOrigin, ref Point scanAreaStart, ref Point scanAreaEnd, int explosionRange, out bool causedShockwaves)
		{
			causedShockwaves = false;
			int num = 4;
			for (int i = scanAreaStart.X; i <= scanAreaEnd.X; i++)
			{
				for (int j = scanAreaStart.Y; j <= scanAreaEnd.Y; j++)
				{
					if (Vector2.Distance(explosionOrigin, new Vector2((float)(i * 16), (float)(j * 16))) > (float)explosionRange)
					{
						continue;
					}
					Tile tileSafely = Framing.GetTileSafely(i, j);
					if (!tileSafely.IsActive || !Main.tileSolid[tileSafely.type] || Main.tileSolidTop[tileSafely.type] || Main.tileFrameImportant[tileSafely.type])
					{
						continue;
					}
					Tile tileSafely2 = Framing.GetTileSafely(i, j - 1);
					if (tileSafely2.IsActive && Main.tileSolid[tileSafely2.type] && !Main.tileSolidTop[tileSafely2.type])
					{
						continue;
					}
					int num2 = WorldGen.KillTile_GetTileDustAmount(fail: true, tileSafely, i, j) * dustAmountMultiplier;
					for (int k = 0; k < num2; k++)
					{
						Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
						dust.velocity.Y -= 3f + (float)num * 1.5f;
						dust.velocity.Y *= Main.rand.NextFloat();
						dust.scale += (float)num * 0.03f;
					}
					if (num >= 2)
					{
						for (int l = 0; l < num2 - 1; l++)
						{
							Dust dust2 = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
							dust2.velocity.Y -= 1f + (float)num;
							dust2.velocity.Y *= Main.rand.NextFloat();
						}
					}
					if (num2 > 0)
					{
						causedShockwaves = true;
					}
				}
			}
		}

		private void CreateImpactExplosion2_FlailTileCollision(Vector2 explosionOrigin, bool causedShockwaves, Vector2 velocityBeforeCollision)
		{
			Vector2 spinningpoint = new(7f, 0f);
			Vector2 val = new(1f, 0.7f);
			Color color = Color.White * 0.5f;
			Vector2 val2 = velocityBeforeCollision.SafeNormalize(Vector2.Zero);
			for (float num = 0f; num < 8f; num += 1f)
			{
				Vector2 val3 = spinningpoint.RotatedBy(num * ((float)Math.PI * 2f) / 8f) * val;
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
				dust.alpha = 0;
				if (!causedShockwaves)
				{
					dust.alpha = 50;
				}
				dust.color = color;
				dust.position = explosionOrigin + val3;
				dust.velocity.Y -= 0.8f;
				dust.velocity.X *= 0.8f;
				dust.fadeIn = 0.3f + Main.rand.NextFloat() * 0.4f;
				dust.scale = 0.4f;
				dust.noLight = true;
				dust.velocity += val2 * 2f;
			}
			if (!causedShockwaves)
			{
				for (float num2 = 0f; num2 < 8f; num2 += 1f)
				{
					Vector2 val4 = spinningpoint.RotatedBy(num2 * ((float)Math.PI * 2f) / 8f) * val;
					Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
					dust2.alpha = 100;
					dust2.color = color;
					dust2.position = explosionOrigin + val4;
					dust2.velocity.Y -= 1f;
					dust2.velocity.X *= 0.4f;
					dust2.fadeIn = 0.3f + Main.rand.NextFloat() * 0.4f;
					dust2.scale = 0.4f;
					dust2.noLight = true;
					dust2.velocity += val2 * 1.5f;
				}
			}
		}
	}
}

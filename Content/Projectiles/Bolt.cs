using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CurseOfTheMoon.Content.Projectiles
{
    public class Bolt : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bolt"); // Name of the projectile. It can be appear in chat
		}

		public override void SetDefaults()
		{
			Projectile.width = 8; // The width of projectile hitbox
			Projectile.height = 8; // The height of projectile hitbox

			Projectile.aiStyle = 0;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
			Projectile.tileCollide = false; 
			Projectile.timeLeft = 600;
			Projectile.alpha = 127;
		}

		// Custom AI
		public override void AI()
		{
			float num185 = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);
			if (Projectile.localAI[0] == 0f)
			{
				Projectile.localAI[0] = num185;
			}
			float num187 = Projectile.position.X;
			float num188 = Projectile.position.Y;
			float num189 = 300f;
			bool flag4 = false;
			int num190 = 0;
			if (Projectile.ai[1] == 0f)
			{
				for (int num191 = 0; num191 < 200; num191++)
				{
					if (Main.npc[num191].CanBeChasedBy(this) && (Projectile.ai[1] == 0f || Projectile.ai[1] == (float)(num191 + 1)))
					{
						float num192 = Main.npc[num191].position.X + (float)(Main.npc[num191].width / 2);
						float num193 = Main.npc[num191].position.Y + (float)(Main.npc[num191].height / 2);
						float num194 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num192) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num193);
						if (num194 < num189 && Collision.CanHit(new Vector2(Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2)), 1, 1, Main.npc[num191].position, Main.npc[num191].width, Main.npc[num191].height))
						{
							num189 = num194;
							num187 = num192;
							num188 = num193;
							flag4 = true;
							num190 = num191;
						}
					}
				}
				if (flag4)
				{
					Projectile.ai[1] = num190 + 1;
				}
				flag4 = false;
			}
			if (Projectile.ai[1] > 0f)
			{
				int num195 = (int)(Projectile.ai[1] - 1f);
				if (Main.npc[num195].active && Main.npc[num195].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[num195].dontTakeDamage)
				{
					float num196 = Main.npc[num195].position.X + (float)(Main.npc[num195].width / 2);
					float num197 = Main.npc[num195].position.Y + (float)(Main.npc[num195].height / 2);
					if (Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num196) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num197) < 1000f)
					{
						flag4 = true;
						num187 = Main.npc[num195].position.X + (float)(Main.npc[num195].width / 2);
						num188 = Main.npc[num195].position.Y + (float)(Main.npc[num195].height / 2);
					}
				}
				else
				{
					Projectile.ai[1] = 0f;
				}
			}
			if (!Projectile.friendly)
			{
				flag4 = false;
			}
			if (flag4)
			{
				Vector2 val34 = new(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
				float num199 = num187 - val34.X;
				float num200 = num188 - val34.Y;
				float num201 = (float)Math.Sqrt(num199 * num199 + num200 * num200);
				num201 = Projectile.localAI[0] / num201;
				num199 *= num201;
				num200 *= num201;
				int num202 = 8;
				if (Projectile.type == 837)
				{
					num202 = 32;
				}
				Projectile.velocity.X = (Projectile.velocity.X * (float)(num202 - 1) + num199) / (float)num202;
				Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num202 - 1) + num200) / (float)num202;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 15;
			}

			if (Projectile.tileCollide == false && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
				Projectile.tileCollide = true;
			}
			if (Main.rand.NextFloat() <= 0.8)
			{
				Dust dust;
				Vector2 pos = Projectile.Center;
				dust = Main.dust[Terraria.Dust.NewDust(pos, 0, 0, DustID.Shadowflame, 0f, 0f, 0, new Color(255, 255, 255), 1f)];
				dust.noGravity = true;
			}
		}
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
		public override void Kill(int timeLeft)
		{
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		}
	}
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CurseOfTheMoon.Content.Projectiles.Flails.Maces
{
    public class WitheringMace : CotmFlail
	{
		public override void ChainTexture()
		{
			chainTexturePath.Add((GetType().Namespace + "." + Name + "Chain1").Replace('.', '/'));
			chainTexturePath.Add((GetType().Namespace + "." + Name + "Chain1").Replace('.', '/'));
			chainTexturePath.Add((GetType().Namespace + "." + Name + "Chain1").Replace('.', '/'));
			chainTexturePath.Add((GetType().Namespace + "." + Name + "Chain1").Replace('.', '/'));
			chainTexturePath.Add((GetType().Namespace + "." + Name + "Chain0").Replace('.', '/'));
			chainTexturePath.Add((GetType().Namespace + "." + Name + "Chain0").Replace('.', '/'));
			chainTexturePath.Add((GetType().Namespace + "." + "Chain").Replace('.', '/'));
		}
		public override void SetDefaults()
        {
			base.SetDefaults();
			Projectile.netImportant = true;
			Projectile.width = 26; // The width of projectile hitbox
			Projectile.height = 26; // The height of projectile hitbox
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.scale = 1f;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
			Projectile.tileCollide = true;

			maxExtend = 13;
			shotSpeed = 12f;
			returnDistance = 8f;
			returnFast = 13f;
		}

        public override void AI()
        {
            base.AI();
			Dust dust;
			dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f, 100, default, 0.8f)];
			dust.noGravity = true;
			dust.velocity.X *= 2f;
			dust.velocity.Y *= 2f;
			dust.velocity = (dust.velocity + Projectile.velocity) / 2f;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(6) == 0)
			{
				target.AddBuff(BuffID.Slow, 60 * Main.rand.Next(2, 4));
			}
			base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}

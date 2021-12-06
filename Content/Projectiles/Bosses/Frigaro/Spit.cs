using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CurseOfTheMoon.Content.Projectiles.Bosses.Frigaro
{
    public class Spit : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.alpha = 128;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width * 2, Projectile.height * 2, DustID.Water, 0f, 0f, 0, new Color(255, 255, 255), 1.5f)];
            dust.noGravity = true;
            Projectile.velocity.Y += 0.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}

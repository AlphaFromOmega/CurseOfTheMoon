using CurseOfTheMoon.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CurseOfTheMoon.Content.Items.Weapons.Magic
{
	public class Crossbolt : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Temp"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Fires 4 homing bolts");
		}

		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.mana = 20;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 28;
			Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1f;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.noMelee = true;
			Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Bolt>();
			Item.shootSpeed = 5f;
		}

		public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float numberProjectiles = 4;
			float rotation = MathHelper.ToRadians(270);

			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = velocity.RotatedBy((i * rotation / (numberProjectiles - 1)) - rotation/2); // Watch out for dividing by 0 if there is only 1 projectile.
				perturbedSpeed.Normalize();
				Vector2 newPosition = perturbedSpeed * 100f + player.Center;
				Vector2 toMouse = Main.MouseWorld - newPosition;
				toMouse.Normalize();
				for (int j = 0; j < 100; j++)
                {
					if (Main.rand.NextFloat() <= 0.6)
					{
						Dust dust;
						Vector2 pos = newPosition + new Vector2((Main.rand.NextFloat() * 2) - 1, (Main.rand.NextFloat() * 2) - 1);
						dust = Main.dust[Terraria.Dust.NewDust(pos, 0, 0, DustID.Shadowflame, newPosition.X - pos.X, newPosition.Y - pos.Y, 0, new Color(255, 255, 255), 1f)];
						dust.noGravity = true;
					}
				}
				Projectile.NewProjectile(source, newPosition, toMouse * velocity.Length(), type, damage, knockback, player.whoAmI);
			}

			return false; // return false to stop vanilla from calling Projectile.NewProjectile.
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5f, 0f);
		}
	}
}
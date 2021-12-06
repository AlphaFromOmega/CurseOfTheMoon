using CurseOfTheMoon.Content.Projectiles.Flails;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CurseOfTheMoon.Content.Items.Weapons.Melee
{
	public class WeakeningMace : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Tooltip.SetDefault("\'May the blood pour more for all\'");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.knockBack = 4.6f;
			Item.width = 28;
			Item.height = 28;
			Item.damage = 30;
			Item.scale = 1f;
			Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Flails.Maces.WeakeningMace>();
			Item.shootSpeed = 11f;
			Item.UseSound = SoundID.Item1;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 2);
			Item.DamageType = DamageClass.Melee;
			Item.channel = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Mace, 1)
				.AddIngredient(ItemID.CrimtaneBar, 10)
				.AddIngredient(ItemID.CrimsonTorch, 99)
				.Register();
		}

    }
}
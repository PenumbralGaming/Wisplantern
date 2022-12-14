using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using System;
using Terraria.Audio;

namespace Wisplantern.Items.Weapons.Melee.Swords
{
    class DepthrockSledgehammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reinforced Sledgehammer");
            Tooltip.SetDefault("Guarenteed critical strike on uninjured enemies");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.damage = 26;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.knockBack = 9f;
            Item.scale = 1.2f;
            Item.crit = 10;
            Item.value = Item.sellPrice(0, 1, 25, 0);
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Melee;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (target.life == target.lifeMax)
            {
                crit = true;
                Wisplantern.freezeFrames = 5;

                PunchCameraModifier modifier = new PunchCameraModifier(target.Center, new Vector2(Math.Sign(target.Center.X - player.Center.X), 0), 20f, 10f, 10, 1000f);
                Main.instance.CameraModifiers.Add(modifier);

                SoundStyle style = new SoundStyle("Wisplantern/Sounds/Effects/HeavyHit");
                style.MaxInstances = 0;
                style.PitchVariance = 0.5f;
                SoundEngine.PlaySound(style, target.Center);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WeatheredSledgehammer>()
                .AddIngredient<Placeable.Blocks.Depthrock>(75)
                .AddIngredient<Placeable.Blocks.Fulgarite>(15)
                .AddIngredient(ItemID.Ruby, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

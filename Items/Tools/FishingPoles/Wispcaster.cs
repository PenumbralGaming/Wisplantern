﻿using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

namespace Wisplantern.Items.Tools.FishingPoles
{
    class Wispcaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Creates a bobber at your cursor position, within a certain distance");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

            Wisplantern.wisplanternLoot.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);
            Item.width = 54;
            Item.height = 62;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.fishingPole = 25;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<WispcasterBobber>();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        const float maxRange = 300f;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                float distance = player.Distance(Main.MouseWorld);
                if (distance > maxRange) distance = maxRange;
                Vector2 direction = velocity;
                direction.Normalize();

                position = player.Center + direction * distance;
            }
        }
    }

    class WispcasterBobber : ModProjectile
    {
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 61;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            DrawOriginOffsetY = -4;
        }

        public override void ModifyFishingLine(ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineOriginOffset = new Vector2(52, -38);
            lineColor = new Color(187, 206, 238);
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < Main.rand.Next(25, 35); i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<Dusts.HyperstoneDust>())];
                    dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
                }

                SoundStyle style = SoundID.Item9;
                style.MaxInstances = 0;
                SoundEngine.PlaySound(style, Projectile.Center);
            }
        }
    }
}

﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Terraria.Audio;
using System;

namespace Wisplantern.Items.Tools.Movement
{
    class Hooklantern : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Can be thrown an grappled to" +
                "\nOnly two can be active at once");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.noUseGraphic = true;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<HooklanternProjectile>();
            Item.shootSpeed = 15f;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.mana = 20;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool CanUseItem(Player player)
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.owner == player.whoAmI && projectile.active && projectile.type == Item.shoot)
                {
                    count++;
                    if (count >= 2)
                    {
                        return false;
                    }
                }
            }
            return base.CanUseItem(player);
        }
    }

    class HooklanternProjectile : ModProjectile
    {
        public override string Texture => "Wisplantern/Items/Tools/Movement/Hooklantern";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hooklantern");
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
            Projectile.width = 24;
            Projectile.height = 36;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = -10f * Math.Sign(Projectile.velocity.X);
        }

        public override void AI()
        {
            Projectile.rotation *= 0.9f;
            Projectile.velocity *= 0.95f;
            Projectile.ai[0]++;

            Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
            if (player.Distance(Projectile.Center) <= 40f && Projectile.ai[0] > 30)
            {
                Projectile.timeLeft = 0;
                NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.whoAmI);
            }
        }

        public override void Kill(int timeLeft)
        {
            Wisplantern.freezeFrames = 5;
            Wisplantern.freezeFrameLight = true;
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath);
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < Main.rand.Next(25, 35); i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<Dusts.HyperstoneDust>())];
                    dust.velocity = Main.rand.NextVector2Circular(5f, 5f);
                }
            }
        }
    }

    class HooklanternGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        bool hasGrappled = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            hasGrappled = false;
        }

        public override void PostAI(Projectile projectile)
        {
            if (Main.projHook[projectile.type])
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile hooklantern = Main.projectile[i];
                    if (hooklantern != null && hooklantern.type == ModContent.ProjectileType<HooklanternProjectile>() && hooklantern.active && hooklantern.Distance(projectile.Center) <= 20f && hooklantern.ai[0] > 30)
                    {
                        if (!hasGrappled)
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit42, hooklantern.position);
                        }
                        SetGrapple(hooklantern.Center, projectile);
                    }
                }
            }
        }

        public void SetGrapple(Vector2 position, Projectile projectile)
        {
            hasGrappled = true;
            projectile.ai[0] = 2;
            projectile.position = position;
            projectile.position -= projectile.Size / 2;
            Main.player[projectile.owner].grappling[Main.player[projectile.owner].grapCount] = projectile.whoAmI;
            Main.player[projectile.owner].grapCount++;
            projectile.velocity = Vector2.Zero;
            projectile.netUpdate = true;
        }
    }
}

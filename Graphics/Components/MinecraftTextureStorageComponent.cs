using System.Collections.Generic;

using ICSharpCode.SharpZipLib.Zip;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MineLib.PCL.Graphics.Components
{
    public class GUITextures
    {
        // Background folder.
        public Texture2D Panorama0;
        public Texture2D Panorama1;
        public Texture2D Panorama2;
        public Texture2D Panorama3;
        public Texture2D Panorama4;
        public Texture2D Panorama5;

        // Title folder.
        public Texture2D Mojang;
        public Texture2D Minecraft;

        // CreativeInventory folder.
        public Texture2D Tabs;
        public Texture2D TabItems;
        public Texture2D TabItemSearch;
        public Texture2D TabInventory;

        // Container folder.
        public Texture2D Villager;
        public Texture2D StatsIcons;
        public Texture2D Inventory;
        public Texture2D Horse;
        public Texture2D Hopper;
        public Texture2D Generic54;
        public Texture2D Furnace;
        public Texture2D EnchantingTable;
        public Texture2D Dispenser;
        public Texture2D CraftingTable;
        public Texture2D BrewingStand;
        public Texture2D Beacon;
        public Texture2D Anvil;

        // Achievement folder.
        public Texture2D AchievementIcons;
        public Texture2D AchievementBackground;

        // -- Main folder.
        public Texture2D Widgets;
        public Texture2D StreamIndicator;
        public Texture2D ResourcePacks;
        public Texture2D OptionsBackground;
        public Texture2D Icons;
        public Texture2D DemoBackground;
        public Texture2D Book;
    }

    public class MinecraftTextureStorageComponent : GameComponent
    {
        // -- Debugging
        public readonly List<string> UnhandledFiles = new List<string>();
        // -- Debugging

        public GUITextures GUITextures { get; private set; }

        private ZipFile MinecraftJar { get; set; }

        public MinecraftTextureStorageComponent(Game game, ZipFile minecraft) : base(game) { MinecraftJar = minecraft; }

        public void ParseGUITextures()
        {
            GUITextures = new GUITextures();
            foreach (ZipEntry entry in MinecraftJar)
            {
                switch (entry.Name)
                {
                    #region Background folder.
                    case "assets/minecraft/textures/gui/title/background/panorama_0.png":
                        GUITextures.Panorama0 = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/title/background/panorama_1.png":
                        GUITextures.Panorama1 = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/title/background/panorama_2.png":
                        GUITextures.Panorama2 = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/title/background/panorama_3.png":
                        GUITextures.Panorama3 = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/title/background/panorama_4.png":
                        GUITextures.Panorama4 = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/title/background/panorama_5.png":
                        GUITextures.Panorama5 = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;
                    #endregion

                    #region Title folder.
                    case "assets/minecraft/textures/gui/title/mojang.png":
                        GUITextures.Mojang = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/title/minecraft.png":
                        GUITextures.Minecraft = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;
                    #endregion

                    #region CreativeInventory folder.
                    case "assets/minecraft/textures/gui/container/creative_inventory/tabs.png":
                        GUITextures.Tabs = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/creative_inventory/tab_items.png":
                        GUITextures.TabItems = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/creative_inventory/tab_item_search.png":
                        GUITextures.TabItemSearch = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/creative_inventory/tab_inventory.png":
                        GUITextures.TabInventory = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;
                    #endregion

                    #region Container folder.
                    case "assets/minecraft/textures/gui/container/villager.png":
                        GUITextures.Villager = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/stats_icons.png":
                        GUITextures.StatsIcons = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/inventory.png":
                        GUITextures.Inventory = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/horse.png":
                        GUITextures.Horse = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/hopper.png":
                        GUITextures.Hopper = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/generic_54.png":
                        GUITextures.Generic54 = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/furnace.png":
                        GUITextures.Furnace = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/enchanting_table.png":
                        GUITextures.EnchantingTable = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/dispenser.png":
                        GUITextures.Dispenser = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/crafting_table.png":
                        GUITextures.CraftingTable = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/brewing_stand.png":
                        GUITextures.BrewingStand = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/beacon.png":
                        GUITextures.Beacon = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/container/anvil.png":
                        GUITextures.Anvil = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;
                    #endregion

                    #region Achievement folder.
                    case "assets/minecraft/textures/gui/achievement/achievement_icons.png":
                        GUITextures.AchievementIcons = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/achievement/achievement_background.png":
                        GUITextures.AchievementBackground = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;
                    #endregion

                    #region Main folder.
                    case "assets/minecraft/textures/gui/widgets.png":
                        GUITextures.Widgets = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/stream_indicator.png":
                        GUITextures.StreamIndicator = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/resource_packs.png":
                        GUITextures.ResourcePacks = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/options_background.png":
                        GUITextures.OptionsBackground = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/icons.png":
                        GUITextures.Icons = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/demo_background.png":
                        GUITextures.DemoBackground = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;

                    case "assets/minecraft/textures/gui/book.png":
						GUITextures.Book = Texture2D.FromStream(Game.GraphicsDevice, MinecraftJar.GetInputStream(entry));
                        break;
                    #endregion

                    default:
                        if (entry.Name.StartsWith("assets/minecraft/textures/gui"))
                            UnhandledFiles.Add(entry.Name);
                        break;
                }
            }
        }
    }
}
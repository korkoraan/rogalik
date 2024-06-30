using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace rogalik.Rendering;

public static class R
{
    public static ContentManager contentManager;

    public static class Icons
    {
        public static readonly Texture2D map;
        public static readonly Texture2D inventory;
        public static readonly Texture2D abilities;
        public static readonly Texture2D equipment;
        public static readonly Texture2D stats;
        public static readonly Texture2D damage;
        public static readonly Texture2D armorPoints;
        public static readonly Texture2D arrowUpGreen;
        public static readonly Texture2D arrowDownRed;
        public static readonly Texture2D clownfish;

        static Icons()
        {
            inventory = Load("inventory");
            equipment = Load("equipment");
            abilities = Load("abilities");
            stats = Load("stats");
            map = Load("map");
            damage = Load("damage");
            armorPoints = Load("armor_points");
            arrowUpGreen = Load("arrow_up_green");
            arrowDownRed = Load("arrow_down_red");
            clownfish = Load("clownfish");
        }

        public static class Abilities
        {
            public static readonly Texture2D deadlySneeze;
            public static readonly Texture2D devour;
            public static readonly Texture2D fireball;
            public static readonly Texture2D sneak;
            public static readonly Texture2D hellFire;
            public static readonly Texture2D frostBeam;
            public static readonly Texture2D shieldGuard;
            public static readonly Texture2D mightyKick;
            public static readonly Texture2D disarm;

            static Abilities()
            {
                deadlySneeze = Load("deadly_sneeze");
                devour = Load("devour");
                fireball = Load("fireball");
                sneak = Load("sneak");
                hellFire = Load("hell_fire");
                frostBeam = Load("frost_beam");
                shieldGuard = Load("shield_guard");
                mightyKick = Load("mighty_kick");
                disarm = Load("disarm");
            }
            
            private static Texture2D Load(string assetName)
            {
                var path = "icons/" + assetName;
                return contentManager.Load<Texture2D>(path);
            }
        }

        public static class Items
        {
            public static readonly Texture2D armorIron;
            public static readonly Texture2D armorLeather;
            public static readonly Texture2D armorPlate;
            public static readonly Texture2D helmetIron;
            public static readonly Texture2D helmetLeather;
            public static readonly Texture2D helmetPlate;
            public static readonly Texture2D glovesLeather;
            public static readonly Texture2D glovesIron;
            public static readonly Texture2D sword;
            public static readonly Texture2D staff;

            static Items()
            {
                armorIron = Load("armor_iron");
                armorLeather = Load("armor_leather");
                armorPlate = Load("armor_plate");
                helmetLeather = Load("helmet_leather");
                helmetIron = Load("helmet_iron");
                helmetPlate = Load("helmet_plate");
                glovesIron = Load("gloves_iron");
                glovesLeather = Load("gloves_leather");
                sword = Load("sword");
                staff = Load("staff");
            }
        }

        public static class Stats
        {
            public static readonly Texture2D will;
            public static readonly Texture2D agility;
            public static readonly Texture2D strength;
            static Stats()
            {
                will = Load("will");
                agility = Load("agility");
                strength = Load("strength");
            }
        }

        private static Texture2D Load(string assetName)
        {
            var path = "icons/" + assetName;
            return contentManager.Load<Texture2D>(path);
        }
    }

    /// <summary>
    /// To add your tile texture declare a static field with a list of textures or a single texture.
    /// Then in a constructor specify tile name.
    /// To add multiple variants of a tile end their names with _1, _2, _3 etc.
    /// </summary>
    public static class Tiles
    {
        public static readonly Texture2D playerWarrior;
        public static readonly Texture2D goblinUnarmed;
        public static readonly Texture2D spirit;
        public static readonly Texture2D stoneGolem;
        public static readonly Texture2D treant;
        public static readonly Texture2D cat;
        public static readonly List<Texture2D> surfaceRock;
        public static readonly List<Texture2D> wallRock;
        public static readonly List<Texture2D> door;
        public static readonly Texture2D doorOpened;
        public static readonly Texture2D skullAndBones;
        public static readonly Texture2D ironHelmet;
        public static readonly Texture2D plateArmor;
        public static readonly Texture2D sword;
        public static readonly Texture2D shield;

        static Tiles()
        {
            playerWarrior = Load("player_warrior");
            surfaceRock = Load("surface_rock", 4);
            goblinUnarmed = Load("goblin_unarmed");
            spirit = Load("spirit");
            wallRock = Load("wall_rock", 2);
            door = Load("door", 3);
            doorOpened = Load("door_opened");
            stoneGolem = Load("stone_golem");
            treant = Load("treant");
            cat = Load("cat");
            skullAndBones = Load("skull_and_bones");
            ironHelmet = Load("iron_helmet");
            plateArmor = Load("plate_armor");
            sword = Load("sword");
            shield = Load("shield");
        }
        private static Texture2D Load(string assetName)
        {
            var path = "tiles/" + assetName;
            return contentManager.Load<Texture2D>(path);
        }
        
        private static List<Texture2D> Load(string assetName, int variants)
        {
            var path = "tiles/" + assetName;
            var textures = new List<Texture2D>();
            for (var i = 1; i <= variants; i++)
            {
                textures.Add(contentManager.Load<Texture2D>(path + "_" + i));
            }

            return textures;
        }
    }

    public static class UI
    {
        public static Texture2D whitePixel;
        static UI()
        {
            whitePixel = contentManager.Load<Texture2D>("white_pixel");
        }
    }

    public static class Images
    {
        public static readonly Texture2D deathScreen;

        static Images()
        {
            deathScreen = Load("death_screen");
        }
        private static Texture2D Load(string assetName)
        {
            var path = "pictures/" + assetName;
            return contentManager.Load<Texture2D>(path);
        }
    }
}
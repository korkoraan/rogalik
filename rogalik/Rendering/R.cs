using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace rogalik.Rendering;

public static class R
{
    public static ContentManager contentManager;
    
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
        public static readonly List<Texture2D> surfaceRock;
        public static readonly List<Texture2D> wallRock;
        public static readonly List<Texture2D> door;
        public static readonly Texture2D doorOpened;

        static Tiles()
        {
            playerWarrior = Load("player_warrior");
            surfaceRock = Load("surface_rock", 4);
            goblinUnarmed = Load("goblin_unarmed");
            spirit = Load("spirit");
            wallRock = Load("wall_rock", 2);
            door = Load("door", 3);
            doorOpened = Load("door_opened");
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
        public static Dictionary<char, Texture2D> charToTexture = new ();
        static UI()
        {
            whitePixel = contentManager.Load<Texture2D>("white_pixel");
            
            var alphabet ="aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ" ;
            foreach(var c in alphabet)
            {
                var assetName = char.IsLower(c) ? $"{c}" : char.ToLower(c) + "_";
                charToTexture[c] = contentManager.Load<Texture2D>("characters/" + assetName);
            }

            var special = "!$%&(),.'-@[]^_`{}~+=";
            foreach (var c in special)
            {
                charToTexture[c] = contentManager.Load<Texture2D>("characters/" + c);
            }

            var numbers = "0123456789";
            foreach (var c in numbers)
            {
                charToTexture[c] = contentManager.Load<Texture2D>("characters/" + c);
            }
            
            charToTexture['"'] = contentManager.Load<Texture2D>("characters/" + "bracket");
            charToTexture['/'] = contentManager.Load<Texture2D>("characters/" + "slash");
            charToTexture['\\'] = contentManager.Load<Texture2D>("characters/" + "backslash");
            charToTexture[':'] = contentManager.Load<Texture2D>("characters/" + "colon");
            charToTexture['>'] = contentManager.Load<Texture2D>("characters/" + "g_than");
            charToTexture['<'] = contentManager.Load<Texture2D>("characters/" + "l_than");
            charToTexture['?'] = contentManager.Load<Texture2D>("characters/" + "question_mark");
            charToTexture['#'] = contentManager.Load<Texture2D>("characters/" + "sharp");
            charToTexture['*'] = contentManager.Load<Texture2D>("characters/" + "star");
            charToTexture[';'] = contentManager.Load<Texture2D>("characters/" + "semicolon");
        }
    }
}
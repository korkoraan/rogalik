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

        static Tiles()
        {
            playerWarrior = Load("player_warrior");
            surfaceRock = Load("surface_rock", 4);
            goblinUnarmed = Load("goblin_unarmed");
            spirit = Load("spirit");
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
}
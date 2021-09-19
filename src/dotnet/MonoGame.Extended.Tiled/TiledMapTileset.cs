using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Tiled
{
    public interface ITileset
    {
        int ActualWidth { get; }
        int Columns { get; }
        int ActualHeight { get; }
        int Rows { get; }
        int TileWidth { get; }
        int TileHeight { get; }
        int TileCount { get; }
        int Spacing { get; }
        int Margin { get; }
        bool HasSharedTexture { get; }
        Texture2D Texture { get; }
        Texture2D NormalTexture { get; }
        List<TiledMapTilesetTile> Tiles { get; }
        TiledMapProperties Properties { get; }
        Texture2D GetTileTexture(int localId);
        TextureRegion2D GetRegion(int column, int row);
    }

    public class TiledMapCollectionTileset : ITileset
    {
        private Dictionary<int, Texture2D> _texureDict;

        public TiledMapCollectionTileset(Dictionary<int, Texture2D> textureDict, string name,
            int tileWidth, int tileHeight, int tileCount, int spacing, int margin, int columns)
        {
            _texureDict = textureDict;
            Name = name;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            TileCount = tileCount;
            Spacing = spacing;
            Margin = margin;
            Columns = columns;
            Properties = new TiledMapProperties();
            Tiles = new List<TiledMapTilesetTile>();
        }

        public string Name { get; }
        public Texture2D Texture => throw new NotImplementedException();
        public Texture2D NormalTexture => throw new NotImplementedException();

        public TextureRegion2D GetRegion(int column, int row)
        {
            throw new NotImplementedException();
            //var x = Margin + column * (TileWidth + Spacing);
            //var y = Margin + row * (TileHeight + Spacing);
            //return new TextureRegion2D(Texture, x, y, TileWidth, TileHeight);
        }

        public int TileWidth { get; }
        public int TileHeight { get; }
        public int Spacing { get; }
        public int Margin { get; }
        public int TileCount { get; }
        public int Columns { get; }
        public List<TiledMapTilesetTile> Tiles { get; }
        public TiledMapProperties Properties { get; }

        public int Rows => (int)Math.Ceiling((double)TileCount / Columns);
        public int ActualWidth => TileWidth * Columns;
        public int ActualHeight => TileHeight * Rows;
        public bool HasSharedTexture => false;

        public Rectangle GetTileRegion(int localTileIdentifier)
        {
            return TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, TileWidth, TileHeight, Columns, Margin, Spacing);
        }
        public Texture2D GetTileTexture(int localId)
        {
            return _texureDict[localId];
        }
    }

    public class TiledMapTileset : ITileset
    {
        public TiledMapTileset(Texture2D texture, Texture2D normalTexture,
            int tileWidth, int tileHeight, int tileCount, int spacing, int margin, int columns)
        {
            Texture = texture;
            NormalTexture = normalTexture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            TileCount = tileCount;
            Spacing = spacing;
            Margin = margin;
            Columns = columns;
            Properties = new TiledMapProperties();
            Tiles = new List<TiledMapTilesetTile>();
        }

        public string Name => Texture.Name;
        public Texture2D Texture { get; }
        public Texture2D NormalTexture { get; }

        public TextureRegion2D GetRegion(int column, int row)
        {
            var x = Margin + column * (TileWidth + Spacing);
            var y = Margin + row * (TileHeight + Spacing);
            return new TextureRegion2D(Texture, x, y, TileWidth, TileHeight);
        }

        public int TileWidth { get; }
        public int TileHeight { get; }
        public int Spacing { get; }
        public int Margin { get; }
        public int TileCount { get; }
        public int Columns { get; }
        public List<TiledMapTilesetTile> Tiles { get; }
        public TiledMapProperties Properties { get; }

        public int Rows => (int)Math.Ceiling((double) TileCount / Columns);
        public int ActualWidth => TileWidth * Columns;
        public int ActualHeight => TileHeight * Rows;
        public bool HasSharedTexture => true;

        public Rectangle GetTileRegion(int localTileIdentifier)
        {
            return TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, TileWidth, TileHeight, Columns, Margin, Spacing);
        }
        public Texture2D GetTileTexture(int localId)
        {
            throw new NotImplementedException();
        }
    }
}

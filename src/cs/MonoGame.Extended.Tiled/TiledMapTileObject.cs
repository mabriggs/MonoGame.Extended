using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tiled
{
    public sealed class TiledMapTileObject : TiledMapObject
    {
        public readonly uint GlobalTileIdentifierWithFlags;

        public TiledMapTileObject(uint globalTileIdentifierWithFlags, int identifier, string name, ITileset tileset, TiledMapTilesetTile tile, 
            Size2 size, Vector2 position, float rotation = 0, float opacity = 1, bool isVisible = true, string type = null) 
            : base(identifier, name, size, position, rotation, opacity, isVisible, type)
        {
            GlobalTileIdentifierWithFlags = globalTileIdentifierWithFlags;
            Tileset = tileset;
            Tile = tile;
        }

        public int GlobalIdentifier => (int)(GlobalTileIdentifierWithFlags & ~(uint)TiledMapTileFlipFlags.All);
        public bool IsFlippedHorizontally => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipHorizontally) != 0;
        public bool IsFlippedVertically => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipVertically) != 0;
        public bool IsFlippedDiagonally => (GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.FlipDiagonally) != 0;
        public bool IsBlank => GlobalIdentifier == 0;
        public TiledMapTileFlipFlags Flags => (TiledMapTileFlipFlags)(GlobalTileIdentifierWithFlags & (uint)TiledMapTileFlipFlags.All);

        public TiledMapTilesetTile Tile { get; }
        public ITileset Tileset { get; }
    }
}

﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Tiled
{
	public class TiledMapTilesetReader : ContentTypeReader<ITileset>
	{
		protected override ITileset Read(ContentReader reader, ITileset existingInstance)
		{
			if (existingInstance != null)
				return existingInstance;

			return ReadTileset(reader);
		}

		public static ITileset ReadTileset(ContentReader reader)
		{
            Texture2D texture = null, normalTexture = null;
            Dictionary<int, Texture2D> textureDict = null;
            var tilesetImageFlag = reader.ReadBoolean();
            var tileWidth = reader.ReadInt32();
            var tileHeight = reader.ReadInt32();
            var tileCount = reader.ReadInt32();
            var spacing = reader.ReadInt32();
            var margin = reader.ReadInt32();
            var columns = reader.ReadInt32();
            var explicitTileCount = reader.ReadInt32();
            if (tilesetImageFlag)
                texture = reader.ReadExternalReference<Texture2D>();
            else
            {
                textureDict = new Dictionary<int, Texture2D>();
                for(var i=0; i < explicitTileCount; i++)
                {
                    var tileTexture = reader.ReadExternalReference<Texture2D>();
                    textureDict.Add(i, tileTexture);
                }
            }

            var normalTextureFlag = reader.ReadBoolean();
            if (normalTextureFlag)
            {
               normalTexture = reader.ReadExternalReference<Texture2D>();
            }

            ITileset tileset;
            if(tilesetImageFlag)
                tileset = new TiledMapTileset(texture, normalTexture, tileWidth, tileHeight, tileCount, spacing, margin, columns);
            else
            {
                tileset = new TiledMapCollectionTileset(textureDict, "test", tileWidth, tileHeight, tileCount, spacing, margin, columns);
            }
                

            for (var tileIndex = 0; tileIndex < explicitTileCount; tileIndex++)
            {
                var localTileIdentifier = reader.ReadInt32();
                var type = reader.ReadString();
                var animationFramesCount = reader.ReadInt32();
                var tilesetTile = animationFramesCount <= 0 
                    ? ReadTiledMapTilesetTile(reader, tileset, objects => 
                        new TiledMapTilesetTile(localTileIdentifier, type, objects)) 
                    : ReadTiledMapTilesetTile(reader, tileset, objects => 
                        new TiledMapTilesetAnimatedTile(localTileIdentifier, ReadTiledMapTilesetAnimationFrames(reader, tileset, animationFramesCount), type, objects));

                ReadProperties(reader, tilesetTile.Properties);
                tileset.Tiles.Add(tilesetTile);
            }

            ReadProperties(reader, tileset.Properties);

            return tileset;
		}

		private static TiledMapTilesetTileAnimationFrame[] ReadTiledMapTilesetAnimationFrames(ContentReader reader, ITileset tileset, int animationFramesCount)
		{
			var animationFrames = new TiledMapTilesetTileAnimationFrame[animationFramesCount];

			for (var i = 0; i < animationFramesCount; i++)
			{
				var localTileIdentifierForFrame = reader.ReadInt32();
				var frameDurationInMilliseconds = reader.ReadInt32();
				var tileSetTileFrame = new TiledMapTilesetTileAnimationFrame(tileset, localTileIdentifierForFrame, frameDurationInMilliseconds);
				animationFrames[i] = tileSetTileFrame;
			}

			return animationFrames;
		}

		private static TiledMapTilesetTile ReadTiledMapTilesetTile(ContentReader reader, ITileset tileset, Func<TiledMapObject[], TiledMapTilesetTile> createTile)
		{
			var objectCount = reader.ReadInt32();
			var objects = new TiledMapObject[objectCount];

			for (var i = 0; i < objectCount; i++)
				objects[i] = ReadTiledMapObject(reader, tileset);

			return createTile(objects);
		}

		private static TiledMapObject ReadTiledMapObject(ContentReader reader, ITileset tileset)
		{
			var objectType = (TiledMapObjectType)reader.ReadByte();
			var identifier = reader.ReadInt32();
			var name = reader.ReadString();
			var type = reader.ReadString();
			var position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			var width = reader.ReadSingle();
			var height = reader.ReadSingle();
			var size = new Size2(width, height);
			var rotation = reader.ReadSingle();
			var isVisible = reader.ReadBoolean();
			var properties = new TiledMapProperties();
			const float opacity = 1.0f;

			ReadProperties(reader, properties);

			TiledMapObject mapObject;

			switch (objectType)
			{
				case TiledMapObjectType.Rectangle:
					mapObject = new TiledMapRectangleObject(identifier, name, size, position, rotation, opacity, isVisible, type);
					break;
				case TiledMapObjectType.Tile:
					reader.ReadUInt32(); // Tile objects within TiledMapTilesetTiles currently ignore the gid and behave like rectangle objects.
					mapObject = new TiledMapRectangleObject(identifier, name, size, position, rotation, opacity, isVisible, type);
					break;
				case TiledMapObjectType.Ellipse:
					mapObject = new TiledMapEllipseObject(identifier, name, size, position, rotation, opacity, isVisible);
					break;
				case TiledMapObjectType.Polygon:
					mapObject = new TiledMapPolygonObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible, type);
					break;
				case TiledMapObjectType.Polyline:
					mapObject = new TiledMapPolylineObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible, type);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			foreach (var property in properties)
				mapObject.Properties.Add(property.Key, property.Value);

			return mapObject;
		}

		private static Point2[] ReadPoints(ContentReader reader)
		{
			var pointCount = reader.ReadInt32();
			var points = new Point2[pointCount];

			for (var i = 0; i < pointCount; i++)
			{
				var x = reader.ReadSingle();
				var y = reader.ReadSingle();
				points[i] = new Point2(x, y);
			}

			return points;
		}

		private static void ReadProperties(ContentReader reader, TiledMapProperties properties)
		{
			var count = reader.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var key = reader.ReadString();
				var value = reader.ReadString();
				properties[key] = value;
			}
		}
	}
}

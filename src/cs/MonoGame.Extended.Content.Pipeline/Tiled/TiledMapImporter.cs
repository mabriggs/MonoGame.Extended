﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tiled.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentImporter(".tmx", DefaultProcessor = "TiledMapProcessor", DisplayName = "Tiled Map Importer - MonoGame.Extended")]
    public class TiledMapImporter : ContentImporter<TiledMapContentItem>
    {
        public override TiledMapContentItem Import(string filePath, ContentImporterContext context)
        {
            try
            {
                if (filePath == null)
                    throw new ArgumentNullException(nameof(filePath));

                ContentLogger.Logger = context.Logger;
                ContentLogger.Log($"Importing '{filePath}'");

                var map = DeserializeTiledMapContent(filePath, context);

                if (map.Width > ushort.MaxValue || map.Height > ushort.MaxValue)
                    throw new InvalidContentException($"The map '{filePath} is much too large. The maximum supported width and height for a Tiled map is {ushort.MaxValue}.");

                ContentLogger.Log($"Imported '{filePath}'");

                return new TiledMapContentItem(map);

            }
            catch (Exception e)
            {
                context.Logger.LogImportantMessage(e.StackTrace);
				throw;
            }
        }

        private static TiledMapContent DeserializeTiledMapContent(string mapFilePath, ContentImporterContext context)
        {
            using (var reader = new StreamReader(mapFilePath))
			{
				var mapSerializer = new XmlSerializer(typeof(TiledMapContent));
				var map = (TiledMapContent)mapSerializer.Deserialize(reader);

				map.FilePath = mapFilePath;

				for (var i = 0; i < map.Tilesets.Count; i++)
				{
					var tileset = map.Tilesets[i];

					string getTilesetSource(string source)
						=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(mapFilePath), source));

					if (!string.IsNullOrWhiteSpace(tileset.Source))
					{
                        tileset.Source = getTilesetSource(tileset.Source);
                        ContentLogger.Log($"Adding dependency for {tileset.Source}");
						// We depend on the tileset. If the tileset changes, the map also needs to rebuild.
						context.AddDependency(tileset.Source);
					}
					else if(tileset.Image != null)
					{
                        tileset.Image.Source = getTilesetSource(tileset.Image.Source);
						ContentLogger.Log($"Adding dependency for {tileset.Image.Source}");
						context.AddDependency(tileset.Image.Source);

                        var normalProp = tileset.Properties.FirstOrDefault(p => p.Name == TiledMapTilesetWriter.NormalTilesetPropertyName);
                        if (normalProp != null)
                        {
                            normalProp.ValueAttribute = getTilesetSource(normalProp.ValueAttribute);
                            ContentLogger.Log($"Adding dependency for {normalProp.ValueAttribute}");
                            context.AddDependency(normalProp.ValueAttribute);
                        }
					}
                    else
                    {
                        // TODO
                    }
				}

				ImportLayers(context, map.Layers, Path.GetDirectoryName(mapFilePath));

				map.Name = mapFilePath;
				return map;
			}
		}

		private static void ImportLayers(ContentImporterContext context, List<TiledMapLayerContent> layers, string path)
		{
			for (var i = 0; i < layers.Count; i++)
			{
				if (layers[i] is TiledMapImageLayerContent imageLayer)
				{
                    imageLayer.Image.Source = Path.Combine(path, imageLayer.Image.Source);
					ContentLogger.Log($"Adding dependency for '{imageLayer.Image.Source}'");

					// Tell the pipeline that we depend on this image and need to rebuild the map if the image changes.
					// (Maybe the image is a different size)
					context.AddDependency(imageLayer.Image.Source);
				}
				if (layers[i] is TiledMapObjectLayerContent objectLayer)
					foreach (var obj in objectLayer.Objects)
						if (!String.IsNullOrWhiteSpace(obj.TemplateSource))
						{
                            obj.TemplateSource = Path.Combine(path, obj.TemplateSource);
                            ContentLogger.Log($"Adding dependency for '{obj.TemplateSource}'");
							// Tell the pipeline that we depend on this template and need to rebuild the map if the template changes.
							// (Templates are loaded into objects on process, so all objects which depend on the template file
							//  need the change to the template)
							context.AddDependency(obj.TemplateSource);
						}
				if (layers[i] is TiledMapGroupLayerContent groupLayer)
					// Yay recursion!
					ImportLayers(context, groupLayer.Layers, path);
			}
		}
    }
}

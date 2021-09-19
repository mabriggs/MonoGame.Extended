using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled.Serialization;
using System.Linq;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentImporter(".tsx", DefaultProcessor = "TiledMapTilesetProcessor", DisplayName = "Tiled Map Tileset Importer - MonoGame.Extended")]
	public class TiledMapTilesetImporter : ContentImporter<TiledMapTilesetContentItem>
	{
		public override TiledMapTilesetContentItem Import(string filePath, ContentImporterContext context)
		{
			try
			{
				if (filePath == null)
					throw new ArgumentNullException(nameof(filePath));

				ContentLogger.Logger = context.Logger;
				ContentLogger.Log($"Importing '{filePath}'");

				var tileset = DeserializeTiledMapTilesetContent(filePath, context);

				ContentLogger.Log($"Imported '{filePath}'");

				return new TiledMapTilesetContentItem(tileset);
			}
			catch (Exception e)
			{
				context.Logger.LogImportantMessage(e.StackTrace);
				throw;
			}
		}

		private TiledMapTilesetContent DeserializeTiledMapTilesetContent(string filePath, ContentImporterContext context)
		{
			using (var reader = new StreamReader(filePath))
			{
				var tilesetSerializer = new XmlSerializer(typeof(TiledMapTilesetContent));
				var tileset = (TiledMapTilesetContent)tilesetSerializer.Deserialize(reader);

                if(tileset.Image != null)
                {
                    tileset.Image.Source = Path.Combine(Path.GetDirectoryName(filePath), tileset.Image.Source);
                    ContentLogger.Log($"Adding dependency '{tileset.Image.Source}'");
                    context.AddDependency(tileset.Image.Source);
                }

                var normalProp = tileset.Properties.FirstOrDefault(p => p.Name == TiledMapTilesetWriter.NormalTilesetPropertyName);
                if (normalProp != null)
                {
                    normalProp.ValueAttribute = Path.Combine(Path.GetDirectoryName(filePath), normalProp.ValueAttribute);
                    ContentLogger.Log($"Adding dependency '{normalProp.ValueAttribute}'");
                    context.AddDependency(normalProp.ValueAttribute);
                }
                    
				foreach (var tile in tileset.Tiles)
				{
                    if(tile.Image != null)
                    {
                        tile.Image.Source = Path.Combine(Path.GetDirectoryName(filePath), tile.Image.Source);
                        ContentLogger.Log($"Adding dependency '{tile.Image.Source}'");
                        context.AddDependency(tile.Image.Source);
                    }

				    foreach (var obj in tile.Objects)
				    {
				        if (!string.IsNullOrWhiteSpace(obj.TemplateSource))
				        {
				            obj.TemplateSource = Path.Combine(Path.GetDirectoryName(filePath), obj.TemplateSource);
                            ContentLogger.Log($"Adding dependency '{obj.TemplateSource}'");

				            // We depend on the template.
				            context.AddDependency(obj.TemplateSource);
				        }
				    }
				}

			    return tileset;
			}
		}
	}
}

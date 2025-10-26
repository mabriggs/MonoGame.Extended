// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Tiled;

public class TiledMapTilesetTileContent
{
    public TiledMapTilesetTileContent()
    {
        Properties = new List<TiledMapPropertyContent>();
        Type = string.Empty;
    }

    [XmlAttribute(AttributeName = "id")]
    public int LocalIdentifier { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }

    [XmlElement(ElementName = "image")]
    public TiledMapImageContent Image { get; set; }

    [XmlElement(ElementName = "normalimage")]
    public TiledMapImageContent NormalImage { get; set; }

    [XmlElement(ElementName = "heightmapimage")]
    public TiledMapImageContent HeightMapImage { get; set; }

    [XmlArray("objectgroup")]
    [XmlArrayItem("object")]
    public List<TiledMapObjectContent> Objects { get; set; }

    [XmlArray("animation")]
    [XmlArrayItem("frame")]
    public List<TiledMapTilesetTileAnimationFrameContent> Frames { get; set; }

    [XmlArray("properties")]
    [XmlArrayItem("property")]
    public List<TiledMapPropertyContent> Properties { get; set; }

    public override string ToString()
    {
        return LocalIdentifier.ToString();
    }
}

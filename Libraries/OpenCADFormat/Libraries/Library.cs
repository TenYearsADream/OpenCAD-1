﻿using System;
using System.Xml.Serialization;

using OpenCAD.OpenCADFormat.MetaInformation;

namespace OpenCAD.OpenCADFormat.Libraries
{
    [Serializable]
    public abstract class Library
    {
        [XmlAttribute]
        public string Name = "*";

        [XmlAttribute]
        public string Description = "*";

        [XmlArray()]
        [XmlArrayItem("Field")]
        public MetadataFieldCollection Metadata;

        public Library()
        {
            Metadata = new MetadataFieldCollection {
                new MetadataField{ Name = "Comment", Value = "" },
            };
        }
    }
}
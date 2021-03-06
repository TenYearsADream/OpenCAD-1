﻿using OpenCAD.OpenCADFormat.CoordinateSystem;

namespace OpenCAD.OpenCADFormat.Drawing
{
    public class Image : DrawingElement
    {
        public string EmbeddedResourceID { get; private set; }
        public Point TopLeft { get; private set; }
        public Size Size { get; private set; }
    }
}
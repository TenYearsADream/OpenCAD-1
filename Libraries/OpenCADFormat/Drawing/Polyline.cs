﻿using System.Collections.Generic;
using OpenCAD.OpenCADFormat.CoordinateSystem;

namespace OpenCAD.OpenCADFormat.Drawing
{
    public class Polyline : Shape
    {
        public List<Point> Points;

        protected Polyline()
        {
            Points = new List<Point>();
        }
    }
}
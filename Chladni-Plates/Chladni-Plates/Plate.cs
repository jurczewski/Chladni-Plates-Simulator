using System;
using System.Collections.Generic;
using System.Drawing;

namespace Chladni_Plates
{
    public static class Plate
    {
        public static int Size = 440;
        public static int TrianglesInRow => 2 * (Size - 1);
        public static int NumberOfTriangles => TrianglesInRow * (Size - 1);
        public static int N => NumberOfTriangles;

        /// <summary>
        /// Get vertices of i-th triangle
        /// </summary>
        /// <param name="i"></param>
        public static List<Point> GetTrianglePoints(int i)
        {
            if (i < 1 || i > NumberOfTriangles)
            {
                throw new ArgumentException($"Triangle index out of bounds [1, {NumberOfTriangles}]");
            }
            var isLower = i % 2 == 1;
            //i-1 because of 1 based indexing
            var row = (i - 1) / TrianglesInRow;
            var column = ((i - 1) % TrianglesInRow + 1) / 2;

            if (isLower)
            {
                return new List<Point>{
                    new Point(column, row + 1),
                    new Point(column+1, row + 1),
                    new Point(column, row)
                };
            }
            else
            {
                return new List<Point>{
                    new Point(column, row),
                    new Point(column-1, row),
                    new Point(column, row + 1)
                };
            }
        }
    }
}

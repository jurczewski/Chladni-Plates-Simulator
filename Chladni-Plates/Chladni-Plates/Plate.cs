using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace Chladni_Plates
{
    public static class Plate
    {
        public static int Size = 440;
        public static int TrianglesInRow => 2 * (Size - 1);
        public static int NumberOfTriangles => TrianglesInRow * (Size - 1);
        public static int N => NumberOfTriangles;

        public static Matrix<double> Stiffness = Matrix<double>.Build.Dense(3, 3, new double[] { 0, 0, 0, 0, 1, 0, 0, 0, 1 });

        public static Matrix<double> Mass = Matrix<double>.Build.Dense(3, 3, new double[] { 12, 4, 4, 4, 2, 1, 4, 1, 2 });

        public static bool IsLesserEqual(this Point self, Point other)
            => self.X <= other.X || self.Y <= other.Y;

        public static bool IsGreaterEqual(this Point self, Point other)
            => self.X >= other.X || self.Y >= other.Y;

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
                    new Point(column, row + 1, column + Size * (row + 1)),
                    new Point(column + 1, row + 1, column + 1 + Size * (row + 1)),
                    new Point(column, row, column + Size * row)
                };
            }
            else
            {
                return new List<Point>{
                    new Point(column, row, column + Size * row),
                    new Point(column - 1, row, column - 1 + Size * row),
                    new Point(column, row + 1, column + Size * (row + 1))
                };
            }
        }
    }
}

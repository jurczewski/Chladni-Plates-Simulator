using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

namespace Chladni_Plates
{
    public static class Plate
    {
        public static int Size;
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

        public static Vector<double> RunAlgorithm()
        {
            int size = Plate.Size;
            int ppp = size * size;

            var S = Matrix<double>.Build.Dense(ppp, ppp);
            var M = Matrix<double>.Build.Dense(ppp, ppp);

            for (int i = 1; i <= Plate.NumberOfTriangles; i++)
            {
                var trianglePoints = Plate.GetTrianglePoints(i);
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        var valueFromS = S.At(trianglePoints[j].I, trianglePoints[k].I);
                        valueFromS += Plate.Stiffness.At(j, k);
                        S.At(trianglePoints[j].I, trianglePoints[k].I, valueFromS);

                        var valueFromM = M.At(trianglePoints[j].I, trianglePoints[k].I);
                        valueFromM += Plate.Mass.At(j, k);
                        M.At(trianglePoints[j].I, trianglePoints[k].I, valueFromM);
                    }
                }
            }

            Print2DArray(M.ToArray());

            var centerTriagnle = size / 2 * size / 2 + size;
            S = S.RemoveColumn(centerTriagnle).RemoveRow(centerTriagnle);
            M = M.RemoveColumn(centerTriagnle).RemoveRow(centerTriagnle);

            var R = M.Inverse() * S;
            var result = R.Evd();
            return result.EigenVectors.Column(0);
        }
        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}

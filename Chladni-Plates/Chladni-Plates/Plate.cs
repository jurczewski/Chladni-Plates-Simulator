using MathNet.Numerics.LinearAlgebra;
using System;

namespace Chladni_Plates
{
    public class Triangle
    {
        public Point P1, P2, P3;
        public bool IsLower;

        public Triangle(Point p1, Point p2, Point p3, bool isLower)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            IsLower = isLower;
        }

        public Point GetPoint(int i)
        {
            return i switch
            {
                0 => P1,
                1 => P2,
                2 => P3,
                _ => throw new ArgumentException("Index out of bounds."),
            };
        }
    }
    public static class Plate
    {    
        public static int Size;
        public static int TrianglesInRow => 2 * (Size - 1);
        public static int NumberOfTriangles => TrianglesInRow * (Size - 1);
        public static int N => NumberOfTriangles;

        public static double [,] Stiffness = new double[,] { { 2, -1, -1 }, { -1, 1, 0 }, { -1, 0, 1 } };

        public static double [,] Mass = new double[,] { { 2, 1, 1 }, { 1, 2, 1 }, { 1, 1, 2 } };

        public static bool IsLesserEqual(this Point self, Point other)
            => self.X <= other.X || self.Y <= other.Y;

        public static bool IsGreaterEqual(this Point self, Point other)
            => self.X >= other.X || self.Y >= other.Y;

        /// <summary>
        /// Get vertices of i-th triangle
        /// </summary>
        /// <param name="i"></param>
        public static Triangle GetTrianglePoints(int i)
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
                return new Triangle(
                    new Point(column, row + 1, column + Size * (row + 1)),
                    new Point(column + 1, row + 1, column + 1 + Size * (row + 1)),
                    new Point(column, row, column + Size * row),
                    isLower
                );
            }
            else
            {
                return new Triangle(
                    new Point(column, row, column + Size * row),
                    new Point(column - 1, row, column - 1 + Size * row),
                    new Point(column, row + 1, column + Size * (row + 1)),
                    isLower
                );
            }
        }

        public static Matrix<double> RunAlgorithm()
        {
            int size = Plate.Size;
            int ppp = size * size;

            //double[] ss = new double[ppp * ppp];
            //double[] mm = new double[ppp * ppp];
            //for (int i = 1; i <= Plate.NumberOfTriangles; i++)
            //{
            //    var triangles = Plate.GetTrianglePoints(i);
            //    for (int j = 0; j < 3; j++)
            //    {
            //        for (int k = 0; k < 3; k++)
            //        {
            //            double f = triangles.IsLower ? 1 : -1;
            //            int idx = triangles.GetPoint(j).I * ppp + triangles.GetPoint(k).I;
            //            ss[idx] += Plate.Stiffness[j, k] * f;

            //            ss[idx] += Plate.Mass[j, k] * f;
            //        }
            //    }
            //}

            var S = Matrix<double>.Build.Dense(ppp, ppp);
            var M = Matrix<double>.Build.Dense(ppp, ppp);
            
            for (int i = 1; i <= Plate.NumberOfTriangles; i++)
            {
                var triangles = Plate.GetTrianglePoints(i);
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        var valueJI = triangles.GetPoint(j).I;
                        int valueKI = triangles.GetPoint(k).I;

                        var valueFromS = S.At(valueJI, valueKI);
                        valueFromS += Plate.Stiffness[j, k];                        
                        S.At(valueJI, valueKI, valueFromS);

                        var valueFromM = M.At(valueJI, valueKI);
                        valueFromM += Plate.Mass[j, k];
                        M.At(valueJI, valueKI, valueFromM);
                    }
                }
            }           

            var centerTriagnle = size / 2 * (size + 1);
            S = S.RemoveColumn(centerTriagnle).RemoveRow(centerTriagnle);
            M = M.RemoveColumn(centerTriagnle).RemoveRow(centerTriagnle);

            S /= 2.0;
            M /= 24.0;

            var R = M.PseudoInverse() * S;
            var result = R.Evd();

            return result.EigenVectors;            
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

using System;
using System.Collections.Generic;

namespace Chladni_Plates
{
    public static class Algorithm
    {
        public static int Size;
        public static int TrianglesInRow => 2 * (Size - 1);
        public static int TrainglesCount => 2 * (Size - 1) ^ 2;
        public static List<Traingle> Triangles { get; set; }
        public static int VerticesCount => Size ^ 2;
        public static int FixedVertices { get; set; }

        public static double[,] Stiffness = new double[,] {
            { 2, -1, -1 },
            { -1, 1, 0 },
            { -1, 0, 1 } };

        public static double[,] Mass = new double[,] {
            { 2, 1, 1 },
            { 1, 2, 1 },
            { 1, 1, 2 } };

        public static double[,] StiffnessTotal = new double[VerticesCount, VerticesCount];
        public static double[,] MassTotal = new double[VerticesCount, VerticesCount];

        public static double[] EigenValues = new double[VerticesCount - 1];
        public static double[,] EigenVectors = new double[VerticesCount - 1, VerticesCount - 1];

        /// <summary>
        /// Divide plate into triangular mesh, mark fixed vertices
        /// </summary>
        public static void Triangulate()
        {
            for (var i = 0; i < TrainglesCount; i++)
            {
                var isUpper = i % 2;
                var row = i / TrianglesInRow;
                var column = (i % TrianglesInRow) / 2;

                if (isUpper == 1)
                {
                    Triangles.Add(new Traingle(
                        column + 1 + Size * row,
                        column + Size * row,
                        column + 1 + Size * (row + 1)));
                }
                else
                {
                    Triangles.Add(new Traingle(
                        column + Size * (row + 1),
                        column + 1 + Size * (row + 1),
                        column + Size * row));
                }
            }

            var center = Size / 2;
            FixedVertices = center * Size + center;
        }

        public static void FillMatrices()
        {
            foreach (var triangle in Triangles)
            {
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        StiffnessTotal[triangle.Values[i], triangle.Values[j]] += 0.5 * Stiffness[i, j];
                        MassTotal[triangle.Values[i], triangle.Values[j]] += 1 / 24 * MassTotal[i, j];
                    }
                }
            }
        }

        public static void SolveSystem()
        {
            Triangulate();
            FillMatrices();

            var newStiffnessTotal = new double[VerticesCount - 1, VerticesCount - 1];
            var newMassTotal = new double[VerticesCount - 1, VerticesCount - 1];

            for (int i = 0; i < VerticesCount; i++)
            {
                int idx_i = i;
                if (i > FixedVertices)
                {
                    idx_i = i - 1;
                }

                for (int j = 0; j < VerticesCount; j++)
                {
                    int idx_j = j;
                    if (j > FixedVertices)
                    {
                        idx_j = j - 1;
                    }
                    newStiffnessTotal[idx_i, idx_j] = StiffnessTotal[i, j];
                    newMassTotal[idx_i, idx_j] = MassTotal[i, j];
                }
            }

            alglib.smatrixgevd(newStiffnessTotal, VerticesCount - 1, false, newMassTotal, false, 1, 1, out EigenValues, out EigenVectors);
        }

        public static void SetSubmatrix(ref double[,] matrix, int indexI, int indexJ, ref double[,] submatrix)
        {
            for (var i = 0; i < submatrix.GetLength(0); i++)
            {
                for (var j = 0; j < submatrix.GetLength(1); j++)
                {
                    matrix[i + indexI, j + indexJ] = submatrix[i, j];
                }
            }
        }

        public static double[,] IncreaseSize(double[,] matrix, int factor)
        {
            if(matrix.GetLength(0) != matrix.GetLength(1))
            {
                throw new ArgumentException("Current version does not handle non-square matrices");
            }

            var s = matrix.GetLength(0);
            var newS = s + (s - 1) * factor;
            var @out = new double[newS, newS];
            var elementShape = factor + 2;

            for(var i = 0; i < matrix.GetLength(0) - 1; i++)
            {
                for (var j = 0; j < matrix.GetLength(1) - 1; j++)
                {
                    var e = new double[elementShape, elementShape];
                    e[0, 0] = matrix[i, j];
                    e[-1, 0] = matrix[i + 1, j];
                    e[0, -1] = matrix[i, j + 1];
                    e[-1, -1] = matrix[i + 1, j + 1];
                    Interpolate(ref e, 'L');
                    Interpolate(ref e, 'U');
                    SetSubmatrix(ref @out, i * (factor + 1), j * (factor + 1), ref e);
                }
            }            

            return @out;
        }

        public static void Interpolate(ref double[,] matrix, char lu)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1))
            {
                throw new ArgumentException("Matrix not square!");
            }

            var r = matrix.GetLength(0) - 1;
            var c = matrix.GetLength(1) - 1;
            var p1 = new Point(0, 0, matrix[0, 0]);
            var p2 = new Point(r, c, matrix[r, c]);
            Point p3;
            if (lu == 'L')
            {
                p3 = new Point(r, 0, matrix[r, 0]);
            }
            else
            {
                p3 = new Point(0, c, matrix[0, c]);
            }

            var v1 = (p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            var v2 = (p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);

            var A = v1.Item2 * v2.Item3 - v2.Item2 * v1.Item3;
            var B = -v1.Item1 * v2.Item3 + v2.Item1 * v1.Item3;
            var C = v1.Item1 * v2.Item2 - v2.Item1 * v1.Item2;
            var D = -1 * (A * p1.X + B * p1.Y + C * p1.Z);

            //to sie moze wycooorvic
            Func<double, double, double> z = (x, y) => (x * A + y * B + D) / (-C);

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var range_beg = 0;
                var range_end = 0;
                if (lu == 'L')
                {
                    range_beg = 0;
                    range_end = i + 1;
                }
                else
                {
                    range_beg = i;
                    range_end = matrix.GetLength(1);
                }
                for (int j = range_beg; j < range_end; j++)
                {
                    if ((i == p1.X && j == p1.Y) ||
                        (i == p2.X && j == p2.Y) ||
                        (i == p3.X && j == p3.Y))
                    {
                        continue;
                    }

                    matrix[i, j] = z(i, j);
                }
            }
        }
    }
}

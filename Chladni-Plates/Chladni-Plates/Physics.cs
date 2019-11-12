using MathNet.Numerics.LinearAlgebra;
using System.Windows.Media;

namespace Chladni_Plates
{
    public static class Physics
    {
        public static void MultiplicationExample()
        {

            var matrix1 = new Matrix(5, 10, 15, 20, 25, 30);
            var matrix2 = new Matrix(2, 4, 6, 8, 10, 12);

            // matrixResult is equal to (70,100,150,220,240,352) 
            var matrixResult = Matrix.Multiply(matrix1, matrix2);

            // matrixResult2 is also
            // equal to (70,100,150,220,240,352) 
            var matrixResult2 = matrix1 * matrix2;

        }

        //https://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Factorization/Evd%601.htm
        public static void EigenExample()
        {
            Matrix<int> matrix = Matrix<int>.Build.Random(3, 4);
            var eigenvalues = matrix.Evd().EigenValues;
            var eigenvector = matrix.Evd().EigenVectors;
        }
    }
}

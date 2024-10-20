using System;
using System.Threading.Tasks;
using System.Threading;
using MultiThreading.Task3.MatrixMultiplier.Matrices;

namespace MultiThreading.Task3.MatrixMultiplier.Multipliers
{
    public class MatricesMultiplierParallel : IMatricesMultiplier
    {
        public IMatrix Multiply(IMatrix m1, IMatrix m2)
        {
            if (m1.ColCount != m2.RowCount)
            {
                throw new ArgumentException("Invalid matrix dimensions for multiplication.");
            }

            var resultMatrix = MultiplyAllLoopsAsParallel(m1, m2);
            //var resultMatrix = MultiplyOuterAndInnerLoopsAsParallel(m1, m2);
            //var resultMatrix = MultiplyOnlyOuterLoopAsParallel(m1, m2);

            return resultMatrix;
        }

        //Size: 250, Regular: 687 ms, Parallel: 3138 ms
        private static Matrix MultiplyAllLoopsAsParallel(IMatrix m1, IMatrix m2)
        {
            var resultMatrix = new Matrix(m1.RowCount, m2.ColCount);

            Parallel.For(0, m1.RowCount, i =>
            {
                Parallel.For(0, m2.ColCount, j =>
                {
                    long sum = 0;
                    Parallel.For(0, m1.ColCount, k =>
                    {
                        Interlocked.Add(ref sum, m1.GetElement(i, k) * m2.GetElement(k, j));
                    });

                    // THREAD-SAFE ONLY CAUSE INDEX IS UNIQUE FOR EACH WRITE OPERATION
                    resultMatrix.SetElement(i, j, sum);
                });
            });
            return resultMatrix;
        }

        //Size: 250, Regular: 361 ms, Parallel: 103 ms
        private static Matrix MultiplyOuterAndInnerLoopsAsParallel(IMatrix m1, IMatrix m2)
        {
            var resultMatrix = new Matrix(m1.RowCount, m2.ColCount);

            Parallel.For(0, m1.RowCount, i =>
            {
                Parallel.For(0, m2.ColCount, j =>
                {
                    long sum = 0;
                    for (long k = 0; k < m1.ColCount; k++)
                    {
                        sum += m1.GetElement(i, k) * m2.GetElement(k, j);
                    }
                    resultMatrix.SetElement(i, j, sum);
                });
            });
            return resultMatrix;
        }

        //Size: 250, Regular: 273 ms, Parallel: 154 ms
        private static Matrix MultiplyOnlyOuterLoopAsParallel(IMatrix m1, IMatrix m2)
        {
            var resultMatrix = new Matrix(m1.RowCount, m2.ColCount);

            Parallel.For(0, m1.RowCount, i =>
            {
                for (long j = 0; j < m2.ColCount; j++)
                {
                    long sum = 0;
                    for (long k = 0; k < m1.ColCount; k++)
                    {
                        sum += m1.GetElement(i, k) * m2.GetElement(k, j);
                    }
                    resultMatrix.SetElement(i, j, sum);
                }
            });

            return resultMatrix;
        }
    }
}
using System;
using System.Text;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;

namespace AHP.Core
{
    /// <summary>
    /// 层次分析法
    /// AHP教学视频： https://www.bilibili.com/video/BV1hK411G76S?from=search&seid=14234695901164489001
    /// </summary>
    public class AHP
    {
        /// <summary>
        /// 处理数据中保留的小数位
        /// </summary>
        private const int keepFloorBit = 4;
        private static Dictionary<int, double> RI = new Dictionary<int, double>() {
            { 1,0 },{ 2,0 },{ 3,0.52 },{ 4,0.89 },{ 5,1.12 },{ 6,1.26 },{ 7,1.36 },{ 8,1.41 },{ 9,1.46 },{ 10,0.49 },{ 11,0.52 },{ 12,1.54 },{ 13,1.56 },{ 14,1.58 },{ 15,1.59 }
        };
        /// <summary>
        /// 以文本形式展示矩阵
        /// </summary>
        /// <param name="sb"></param>
        private static void DisPlayMatrixToString(StringBuilder sb, Matrix<double> matrix)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                sb.Append($"[");
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    if (j + 1 == matrix.ColumnCount)
                        sb.Append($"{matrix.At(i, j)}");
                    else
                        sb.Append($"{matrix.At(i, j)}\t");
                }
                sb.AppendLine($"]");

            }
        }
        /// <summary>
        /// 矩阵归一化
        /// </summary>
        /// <param name="matrix">矩阵</param>
        /// <param name="sums">各列和</param>
        /// <returns></returns>
        private static Matrix<double> MatrixNormalize(Matrix<double> matrix, double[] sums)
        {
            var normal = Matrix<double>.Build.Dense(matrix.RowCount, matrix.ColumnCount);
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    var v = Math.Round(matrix.At(i, j) / sums[j], keepFloorBit);
                    normal[i, j] = v;
                }
            }
            return normal;
        }
        /// <summary>
        /// 计算Aω
        /// </summary>
        /// <param name="matrix">矩阵</param>
        /// <param name="ws">ω</param>
        /// <returns></returns>
        private static double[] CalcAW(Matrix<double> matrix, double[] ws)
        {
            var res = new double[ws.Length];
            for (int i = 0; i < ws.Length; i++)
            {
                var v = 0.0;
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    v += Math.Round(Convert.ToDouble(matrix.At(i, j)) * ws[j], keepFloorBit);
                }
                res[i] = v;
            }
            return res;
        }
        /// <summary>
        /// 计算λMax
        /// </summary>
        /// <param name="aws">aω</param>
        /// <param name="ws">ω</param>
        /// <returns></returns>
        private static double CalcrMax(double[] aws, double[] ws)
        {
            var v = 0.0;
            for (int i = 0; i < aws.Length; i++)
            {
                v += aws[i] / ws[i];
            }
            return Math.Round(v / aws.Length, keepFloorBit);
        }
        /// <summary>
        /// 开始分析
        /// 采用合积法分析
        /// <paramref name="matrix">传入矩阵</paramref>
        /// </summary>
        public static string Analysis(double[,] matrix)
        {
            var sb = new StringBuilder();
            sb.AppendLine("***************************");
            sb.AppendLine("当前矩阵为：");
            //创建矩阵对象
            var matrixObj = Matrix<double>.Build.DenseOfArray(matrix);
            DisPlayMatrixToString(sb, matrixObj);
            //按列归一化
            //1.得到每一列和
            var sums = matrixObj.ColumnSums().ToArray().Select(c=>Math.Round(c,keepFloorBit)).ToArray();
            sb.AppendLine($"每一列和为：{string.Join(",", sums)}");
            //2.每一列每一个值除以各自列的和
            var matrixNormalObj = MatrixNormalize(matrixObj, sums);
            sb.AppendLine("归一化后的矩阵为：");
            DisPlayMatrixToString(sb, matrixNormalObj);
            //3.ω 即每一行的平均值
            var ws = matrixNormalObj.RowSums().ToArray().Select(c => Math.Round(c / matrixNormalObj.ColumnCount, keepFloorBit)).ToArray();
            sb.AppendLine($"ω为：{string.Join(", ", ws)}");
            //4.计算Aω
            var aws = CalcAW(matrixObj, ws);
            sb.AppendLine($"Aω为：{string.Join(", ", aws)}");
            //5.计算λMax
            var rMax = CalcrMax(aws, ws);
            sb.AppendLine($"λMax为：{rMax}");
            //6.计算CI
            var ci = Math.Round((rMax - matrixNormalObj.RowCount) / (matrixObj.RowCount - 1), keepFloorBit);
            sb.AppendLine($"CI为：{ci}");
            //7.计算RI
            var ri = RI.ContainsKey(matrixNormalObj.RowCount) ? RI[matrixNormalObj.RowCount] : -1;
            sb.AppendLine($"RI为：{ri} 如为-1则当前阶位下RI值不存在 需要查表");
            var cr = Math.Round(ci / ri, keepFloorBit);
            sb.AppendLine($"CR为：{cr}");
            if (cr < 0.1)
                sb.AppendLine($"cr小于0.1通过一致性检验");
            else
                sb.AppendLine($"cr大于0.1未通过一致性检验");
            return sb.ToString();
        }


    }
}

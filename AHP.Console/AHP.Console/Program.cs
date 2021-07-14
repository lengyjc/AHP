using System;

namespace AHP.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("*******************************");
            System.Console.WriteLine("输入多维数组开始分析 采用默认矩阵请直接回车");
            System.Console.WriteLine("目前只支持和积法");
            System.Console.WriteLine("输入允许小数不允许分数，以,分割多个数值");
            System.Console.WriteLine("每一行为完整的一行数据整行数据");
            System.Console.WriteLine("在空值的情况下回车则开始计算");
            System.Console.WriteLine("退出请直接关闭");
            System.Console.WriteLine("*******************************");
            var input = System.Console.ReadLine();
            var msg = string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                msg = Core.AHP.Analysis(new double[4, 4] { { 1, 0.25, 2,0.3333 }, { 4, 1, 8,2 }, { 0.5,  0.125,1,0.2 },{ 3,0.5,5,1} });
               // msg = Core.AHP.Analysis(new double[4, 4] { { 1, 2, 0.3333,3 }, { 0.5, 1, 0.3333,2 }, { 3, 3,1,4 },{ 0.3333,0.5,0.25,1} });
            }
            else
            {
                //首行逻辑
                var row = input.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var matrix = new double[row.Length, row.Length];
                var rowIndex = 0;
                for (int i = 0; i < row.Length; i++)
                {
                    matrix[0, i] = Convert.ToDouble(row[i]);
                }
                rowIndex++;
                do
                {
                    System.Console.WriteLine($"已录入第{rowIndex}行数据,请继续录入");
                    input = System.Console.ReadLine();
                    var rowII = input.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (rowII.Length != row.Length)
                    {
                        System.Console.WriteLine("当前行与第一行列数不同无法进行AHP计算 请重新输入当前行数据");
                        continue;
                    }
                    for (int i = 0; i < rowII.Length; i++)
                    {
                        matrix[rowIndex, i] = Convert.ToDouble(rowII[i]);
                    }
                    rowIndex++;
                } while (!string.IsNullOrEmpty(input));
                if (!(matrix.Rank ==2&& matrix.GetLength(0) == matrix.GetLength(1)))
                {
                    System.Console.WriteLine("行数和列数不同无法进行AHP计算 请重新输入当前行数据");
                    Main(args);
                    return;
                }
                msg = Core.AHP.Analysis(matrix);
            }
            System.Console.WriteLine(msg);
            System.Console.ReadLine();
            //重新开始下一次分析
            Main(args);
        }
    }
}

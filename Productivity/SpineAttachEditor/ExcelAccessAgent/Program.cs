using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace ExcelAccessAgent
{
    class Program
    {
        // 常量定义
        public static ushort ColNameRowId = 2;     // 列名所在行
        public static ushort DataStartRow = 3;     // 数据起始行


        static int Main(string[] args)
        {
            string xlsPath = args[0];
            var readStream = File.OpenRead(xlsPath);
            var xls = new HSSFWorkbook(readStream);
            ISheet sheet = xls.GetSheetAt(0);

            // 解析命令参数
            string mode = args[1];      // read 读取模式 write 写入模式
            string codeVal = args[2];      // Code 搜索值
            // 新的挂点值
            string newAttachStr =  args.Length >=4 ? args[3]: ""; 

            // -----------------------
            // NPOI测试 Dump
            // -----------------------
            //for (int rowId = 0; rowId <= sheet.LastRowNum; rowId++)
            //{
            //    IRow row = sheet.GetRow(rowId);
            //    Console.WriteLine("行号: " + rowId);

            //    for (int colId = row.FirstCellNum; colId < row.LastCellNum; colId++ )
            //    {
            //        Console.WriteLine("列号: " + colId);
            //        ICell cell = row.GetCell(colId);
            //        Console.WriteLine(cell.ToString());         // 注意 - 字符串输出会将公式原样打出来
            //    }
            //}

            // -----------------------
            // NPOI测试 AddOrCreate 测试
            // -----------------------
            //int targetRowId = 19;
            //int targetCellId = 39;
            //IRow targetRow = null;
            //ICell targetCell = null;
            //if (sheet.LastRowNum < targetRowId)
            //    targetRow = sheet.CreateRow(targetRowId);
            //else
            //    targetRow = sheet.GetRow(targetRowId);

            //if (targetRow.LastCellNum < targetCellId)
            //    targetCell = targetRow.CreateCell(targetCellId);
            //else
            //    targetCell = targetRow.GetCell(targetCellId);

            //targetCell.SetCellType(CellType.Formula);
            //targetCell.SetCellFormula("A4+D6");
            //targetCell.SetCellValue();

            // ----------------------------
            // NPOI测试 以一列为Key 查找Cell列表
            // ----------------------------
            List<ICell> cells = FindCells(sheet, "Code", codeVal, "EffectAttach");

            if (cells.Count == 0)
            {
                Console.Error.WriteLine("找不到Code为 {0} 的行", codeVal);
                return -1;
            }

            if (mode == "read")
            {
                Console.Write(cells[0].ToString());
            }

            if (mode == "write")
            {
                Console.WriteLine(newAttachStr);
                foreach (ICell cell in cells)
                {
                    cell.SetCellValue(newAttachStr);
                }

                var writeStream = File.Open(xlsPath, FileMode.Create, FileAccess.Write);
                xls.Write(writeStream);
                writeStream.Close();

            }


            // -----------------------
            // NPOI测试 写文件
            // -----------------------
            //var writeStream = File.Open("output.xls", FileMode.Create, FileAccess.Write);
            //xls.Write(writeStream);
            //writeStream.Close();
                
            // Console.ReadKey();
            return 0;
        }

        // 找到符合条件的单元格
        public static List<ICell> FindCells(ISheet sheet, string keyName, string keyValue, string findName)
        {
            // 搜索Key列
            short keyIndex = 0;
            if (!FindColumnIndex(sheet, keyName, ref keyIndex))
            {
                return new List<ICell>();
            }

            // 搜索Find列
            short findIndex = 0;
            if (!FindColumnIndex(sheet, findName, ref findIndex))
            {
                return new List<ICell>();
            }

            // 找到Key相符行所在的Find Cell
            List<ICell> result = new List<ICell>();
            for (ushort rowId = DataStartRow; rowId <= sheet.LastRowNum; rowId++)
            {
                IRow dataRow = sheet.GetRow(rowId);
                ICell keyCell = dataRow.GetCell(keyIndex);
                if (keyCell != null && keyCell.ToString() == keyValue)
                {
                    ICell findCell = dataRow.GetCell(findIndex);
                    result.Add(findCell);
                }
            }
            return result;
        }

        public static bool FindColumnIndex(ISheet sheet, string columnName, ref short index)
        {
            IRow headRow = sheet.GetRow(ColNameRowId);
            for (short i = headRow.FirstCellNum; i <= headRow.LastCellNum; i++)
            {
                ICell headCell = headRow.GetCell(i);
                string value = headCell.ToString();

                if (headCell.CellType == CellType.String && headCell.ToString() == columnName)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }
}

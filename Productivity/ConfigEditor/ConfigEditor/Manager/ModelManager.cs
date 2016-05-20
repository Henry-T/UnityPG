using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConfigEditor
{
    public class ModelManager
    {
        private static ModelManager instance;
        public static ModelManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ModelManager();
                    instance.Load();
                }
                return instance;
            }
        }

        public WarriorConfig WarriorXlsData;
        public SkillConfig SkillXlsData;
        public XlsData ExpSetXlsData;
        public NatureConfig NatureXlsData;
        public XlsData AvatarXlsData;
        public GradeSetupConfig GradeSetupXlsData;
        public XlsData BuffXlsData;
        public LevelConfig LevelXlsData;
        public LevelNpcConfig LevelNpcXlsData;
        public LevelBgConfig LevelBgXlsData;

        private bool loaded = false;
        public void Load()
        {
            if (!loaded)
                loaded = true;
            else
                return;


            WarriorXlsData = new WarriorConfig("Hero", "ID");
            WarriorXlsData.Load();

            SkillXlsData = new SkillConfig("Skill", 10001, 19999);
            SkillXlsData.Load();

            ExpSetXlsData = new XlsData("ExpSet", "Lv");
            ExpSetXlsData.Load();

            NatureXlsData = new NatureConfig("Nature", "ID");
            NatureXlsData.Load();

            AvatarXlsData = new XlsData("Avatar", "ID");
            AvatarXlsData.Load();

            GradeSetupXlsData = new GradeSetupConfig("GradeSetup", "ID");
            GradeSetupXlsData.Load();

            BuffXlsData = new XlsData("Buff", "ID");
            BuffXlsData.Load();

            LevelXlsData = new LevelConfig("Level", 1001, 9999);
            LevelXlsData.Load();

            LevelNpcXlsData = new LevelNpcConfig("LevelSet", 1001, 9999);
            LevelNpcXlsData.Load();

            LevelBgXlsData = new LevelBgConfig("LevelScene", 1001, 9999);
            LevelBgXlsData.Load();
        }

        //public void Save()
        //{
        //}
    }

    public class XlsData
    {
        public static ushort HeadRowId = 2;
        public static ushort DataStartRowId = 3;

        public String Name;
        public String KeyName;

        public Dictionary<String, int> HeadMap = new Dictionary<string, int>();
        protected HSSFWorkbook xls;
        protected ISheet xlsSheet;

        public DataTable DataTable;

        public XlsData(String name, String keyName)
        {
            Name = name;
            KeyName = keyName;
        }

        protected bool loaded = false;
        public virtual void Load()
        {
            if (loaded)
                return;
            else
                loaded = true;

            string excelDir = UserConfigManager.Instance.Config.ExcelDir;

            DirectoryInfo dirInfo = new DirectoryInfo(excelDir);
            if(!dirInfo.Exists)
            {
                LogManager.Instance.Error("Excel 路径配置错误，自动切换到备用路径，请设置正确路径后重启编辑器");
                excelDir = "Templete/TypeDatas/";
            }

            // 读取 ExpSet.xls
            try
            {
                var readStream = File.Open(Path.Combine(excelDir, Name + ".xls"), FileMode.Open);

                xls = new HSSFWorkbook(readStream);
                readStream.Close();

                xlsSheet = xls.GetSheetAt(0);

                HeadMap.Clear();

                // 第3行是Head行
                IRow headRow = xlsSheet.GetRow(HeadRowId);
                for (short cellId = headRow.FirstCellNum; cellId < headRow.LastCellNum; cellId++)
                {
                    ICell headCell = headRow.GetCell(cellId);
                    if (headCell != null && !String.IsNullOrEmpty(headCell.ToString()))
                    {
                        string key = headCell.ToString();
                        if (HeadMap.ContainsKey(key))
                        {
                            LogManager.Instance.Warn("配置表中存在名称重复的列,已将其忽略,请勿保存该表! " + Name + " " + key);
                        }
                        else
                        {
                            HeadMap.Add(headCell.ToString(), cellId);
                        }
                    }
                    //else
                    //    LogManager.Instance.Log("读取到空单元格: "+);
                }

                DataTable = new DataTable();

                foreach (var pair in HeadMap)
                {
                    DataColumn column = new DataColumn(pair.Key);
                    DataTable.Columns.Add(column);
                    if (pair.Key == KeyName)
                    {
                        DataTable.PrimaryKey = new DataColumn[] { column };
                    }
                }

                int totalCnt = 0;

                for (ushort rowId = DataStartRowId; rowId <= xlsSheet.LastRowNum; rowId++)
                {
                    IRow dataRow = xlsSheet.GetRow(rowId);

                    DataRow row = DataTable.NewRow();

                    foreach (var pair in HeadMap)
                    {
                        ICell cell = dataRow.GetCell(pair.Value);
                        if (cell == null)
                            continue;

                        Object value = null;
                        if (cell.CellType == CellType.Formula)
                        {
                            if (cell.CachedFormulaResultType == CellType.Numeric)
                            {
                                value = cell.NumericCellValue;
                            }
                            else
                            {
                                value = cell.StringCellValue;
                            }
                        }
                        else
                        {
                            value = cell.ToString();
                        }

                        row[pair.Key] = value;
                    }

                    // NOTE thy NPOI 文档说 LastRowNum 是 0-based ，事实上我们的Excel读取后经常多一行
                    if (row[KeyName] != null && !String.IsNullOrEmpty(row[KeyName].ToString()))
                    {
                        totalCnt++;
                        row[KeyName] = int.Parse(row[KeyName].ToString());
                        DataTable.Rows.Add(row);
                    }
                }

                LogManager.Instance.Log(Name + ".xls 转换DataTable完成, 行数: " + totalCnt);

                // 使 DataTableView 按主键排序
                DataTable.DefaultView.Sort = KeyName;
            }
            catch(IOException e)
            {
                String info = "xls 文件被其他程序打开，请关闭后重启编辑器";
                LogManager.Instance.ShowErrorMessageBox(info);
            }
        }

        public DataRowView GetRowViewByKey(String key)
        {
            // NOTE thy DataRow 无法通过Key索引值，必须转换成DataRowView
            DataView dataView = DataTable.AsDataView();
            dataView.RowFilter = KeyName + " = " + key;
            if (dataView.Count == 0)
            {
                return null;
            }
            else
            {
                return dataView[0];
            }
        }

        public virtual void Save()
        {
            // 从第4行开始全部清空
            while (xlsSheet.LastRowNum > HeadRowId)
            {
                IRow lastRow = xlsSheet.GetRow(xlsSheet.LastRowNum);
                xlsSheet.RemoveRow(lastRow);
            }

            List<String> noDefaultCols = new List<string>();

            // 重新写入数据
            foreach (DataRow row in DataTable.Rows)
            {
                IRow xlsRow = xlsSheet.CreateRow(xlsSheet.LastRowNum + 1);
                foreach (var pair in HeadMap)
                {
                    ICell xlsCell = xlsRow.CreateCell(pair.Value, CellType.String);
                    Object value = row[pair.Key];
                    if (value.GetType() == typeof(double))
                        xlsCell.SetCellValue((double)value);
                    else if (value.GetType() == typeof(string))
                        xlsCell.SetCellValue((string)value);
                    else if (value.GetType() == typeof(DBNull))
                    {
                        if (!noDefaultCols.Contains(pair.Key))
                            noDefaultCols.Add(pair.Key);
                        xlsCell.SetCellValue("");
                    }
                    else
                    {                        
                        // Console.WriteLine(value.GetType());
                        xlsCell.SetCellValue((string)value);
                    }
                }
            }

            if (noDefaultCols.Count > 0)
            {
                LogManager.Instance.Warn("以下列未设置列默认值, 请联系编辑器维护人员");
                foreach (String noDefaultCol in noDefaultCols)
                {
                    LogManager.Instance.Log("\t" + Name + " : " + noDefaultCol);
                }
            }

            var writeStream = File.Open(Path.Combine(UserConfigManager.Instance.Config.ExcelDir, Name + ".xls"), FileMode.Create, FileAccess.Write);
            try
            {
                xls.Write(writeStream);
            }
            catch(Exception e)
            {
                // NOTE 这个异常没影响,直接吃掉
                // if (e.GetType() != typeof(ReadingNotSupportedException))
                    throw e;
            }
            writeStream.Close();
        }
    }
}

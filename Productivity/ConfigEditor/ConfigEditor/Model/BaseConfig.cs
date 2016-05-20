using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Reflection;
using NPOI.HPSF;

namespace ConfigEditor
{
    public class BaseConfig<T> where T : BaseType, new()
    {
        public static ushort HeadRowId = 2;
        public static ushort DataStartRowId = 3;

        public int StartID;         // ID 下限
        public int EndID;           // ID 上限

        public String Name;
        public String KeyName;

        protected HSSFWorkbook xls;
        protected ISheet xlsSheet;

        public DataTable DataTable;
        public ObservableCollection<T> DataList;

        public Dictionary<String, int> HeadMap = new Dictionary<string, int>();
        public Dictionary<String, PropertyInfo> PIMap = new Dictionary<string, PropertyInfo>();

        public BaseConfig(String name, int startID, int endID)
        {
            Name = name;
            StartID = startID;
            EndID = endID;
        }

        protected bool loaded = false;
        public void Load()
        {
            if (loaded)
                return;
            else
                loaded = true;

            string excelDir = UserConfigManager.Instance.Config.ExcelDir;
            try
            {
                var readStream = File.Open(Path.Combine(excelDir,  Name + ".xls"), FileMode.Open);

                try
                {
                    xls = new HSSFWorkbook(readStream);
                }
                catch(IOException e)
                {
                    
                }
                finally
                {
                    readStream.Close();
                }

                xlsSheet = xls.GetSheetAt(0);

                DataTable = new DataTable();
                DataList = new ObservableCollection<T>();

                int totalCnt = 0;

                IRow headRow = xlsSheet.GetRow(HeadRowId);
                PropertyInfo[] propInfos = typeof(T).GetProperties();

                for (short cellId = headRow.FirstCellNum; cellId < headRow.LastCellNum; cellId++)
                {
                    ICell headCell = headRow.GetCell(cellId);
                    if (headCell != null && !String.IsNullOrEmpty(headCell.ToString()))
                    {
                        string name = headCell.ToString();
                        HeadMap.Add(name, cellId);
                        IEnumerable<PropertyInfo> findInfos = propInfos.Where(p => p.Name == name);
                        if (findInfos.Count() == 0)
                        {
                            LogManager.Instance.Warn("表中存在的列未在" + Name + "中定义: " + name);
                        }
                        else
                        {
                            PIMap.Add(name, findInfos.First());
                        }
                    }
                    //else
                    //    LogManager.Instance.Log("读取到空单元格: "+);
                }

                foreach (var pair in HeadMap)
                {
                    DataColumn column = new DataColumn(pair.Key);
                    DataTable.Columns.Add(column);
                    if (pair.Key == KeyName)
                    {
                        DataTable.PrimaryKey = new DataColumn[] { column };
                    }
                }

                for (ushort rowId = DataStartRowId; rowId <= xlsSheet.LastRowNum; rowId++)
                {
                    IRow dataRow = xlsSheet.GetRow(rowId);

                    JObject jObject = new JObject();

                    StringBuilder jStrBuilder = new StringBuilder();
                    jStrBuilder.Append("{");

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

                        string valueStr = value.ToString();
                        jStrBuilder.Append("'" + pair.Key + "':'" + value + "',");
                    }
                    jStrBuilder.Append("}");

                    T type = JsonConvert.DeserializeObject<T>(jStrBuilder.ToString());

                    if (type.ID != 0)
                    {
                        DataTable.Rows.Add(type);
                        DataList.Add(type);
                    }
                }

                LogManager.Instance.Log(Name + ".xls 转换DataTable完成, 行数: " + totalCnt);

                // 使 DataTableView 按主键排序
                DataTable.DefaultView.Sort = KeyName;
            }
            catch(IOException e)
            {
                LogManager.Instance.ShowErrorMessageBox("Level.xls被其他程序打开，请关闭它之后重启编辑器。");
            }
        }

        public T GetByID(int id)
        {
            var findResult = DataList.Where(l => l.ID == id);
            if (findResult.Count() == 0)
            {
                return null;
            }
            else
            {
                return findResult.First();
            }
        }

        public T CreateNew()
        {
            // TODO thy 二分法快速查找ID空位

            int insertIndex = 0;
            int newID = 1001;

            for(int i = 0; i<DataList.Count; i++)
            {
                insertIndex = 0;
                if (i == 0)
                {
                    insertIndex = i + 1;
                    newID = DataList[i].ID + 1;
                    if (DataList[i].ID > StartID)
                    {
                        insertIndex = 0;
                        newID = 1001;
                        break;
                    }
                }
                else
                {
                    insertIndex = i + 1;
                    newID = DataList[i].ID + 1;
                    if (DataList[i].ID - DataList[i-1].ID > 1)
                    {
                        insertIndex = i;
                        newID = DataList[i - 1].ID + 1;
                        break;
                    }
                }
            }

            T t = new T();
            t.ID = newID;
            DataList.Insert(insertIndex, t);
            return t;
        }

        public static Level_Type GetLevelEntry(int typeId)
        {
            var findResult = ModelManager.Instance.LevelXlsData.DataList.Where(l => l.ID == typeId);
            if (findResult.Count() == 0)
            {
                return null;
            }
            else
            {
                return findResult.First();
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

            var sortedDataList = DataList.OrderBy(t => t.ID);

            // 重新写入数据
            foreach (T typeData in sortedDataList)
            {
                IRow xlsRow = xlsSheet.CreateRow(xlsSheet.LastRowNum + 1);

                foreach (var pair in PIMap)
                {
                    int cellIndex = this.HeadMap[pair.Key];
                    ICell xlsCell = xlsRow.CreateCell(cellIndex, CellType.String);

                    Object value = pair.Value.GetValue(typeData);

                    if (value == null)
                        continue;

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
                    else if (value.GetType() == typeof(Int32))
                    {
                        xlsCell.SetCellValue(value.ToString());
                    }
                    else
                    {
                        // Console.WriteLine(value.GetType());
                        xlsCell.SetCellValue(value.ToString());
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

            FileStream writeStream = null;
            try
            {
                writeStream = File.Open(Path.Combine(UserConfigManager.Instance.Config.ExcelDir, Name + ".xls"), FileMode.Create, FileAccess.Write);
                try
                {
                    xls.Write(writeStream);
                }
                catch (ReadingNotSupportedException re)
                {
                    // NOTE 这个异常没影响,直接吃掉
                }
                catch (IOException ie)
                {
                    // NOTE 同上NPOI内部错误，吃掉
                }
            }
            catch(IOException ioe)
            {
                LogManager.Instance.ShowErrorMessageBox("配置文件被其他程序占用，请关闭相关程序后重试：" + Name);
            }
            finally
            {
                writeStream.Close();
            }
        }
    }
}

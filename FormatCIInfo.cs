using System;
using System.IO;
using System.Collections.Generic;
 
public class Test
{
  public static void Main()
  {
        string[] folders = Directory.GetDirectories(System.Environment.CurrentDirectory);
        List<String> list = new List<string>();
        foreach (string f in folders)
        {
            string[] f_2 = Directory.GetDirectories(f);
            foreach (string f_2_f in f_2)
            {
                DirectoryInfo dInfo = new DirectoryInfo(f_2_f);
                if (dInfo.GetFiles("ci_info.json").Length > 0)
                {
                    string formatStr = "";
                    formatStr = "<worker Name='" + dInfo.Name + "' ProjectPath='" + dInfo.FullName + "' DefaultCmd=' -command build -platform web' />";
                    list.Add(formatStr);
                }
            }
        }
 
        File.WriteAllLines("result.txt", list.ToArray());
  }
}
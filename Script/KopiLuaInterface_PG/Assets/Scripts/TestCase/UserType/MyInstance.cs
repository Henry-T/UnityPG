using UnityEngine;
using System.Collections;

public class MyInstance
{
    public string Name;
    public void TestFunc()
    {
        Debug.Log("实例成员方法调用成功！");
    }

    public static void StaticFunc()
    {
        Debug.Log("非静态类的静态方法调用成功！");
    }
}
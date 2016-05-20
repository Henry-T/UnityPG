using UnityEngine;
using System.Collections;

public static class MyStatic
{
    public static void TestFunc()
    {
        Debug.Log("静态类的静态方法调用成功！");
    }

    public static string info = "这是一个静态类的静态成员变量";
}
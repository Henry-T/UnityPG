using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ExtensionClass
{
    public static void ExtensionFunc(this EmptyClass obj)
    {
        Debug.Log("扩展方法调用成功！");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MyStaticWrap
{
    public string MyStaticNote 
    {
        get { return MyStatic.info; }
        set { MyStatic.info = value; }
    }

    public void MyStaticTestFunc()
    {
        MyStatic.TestFunc();
    }

    public void MyInstanceStaticFunc()
    {
        MyInstance.StaticFunc();
    }
}
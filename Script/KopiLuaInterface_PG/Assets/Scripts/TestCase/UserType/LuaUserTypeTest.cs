using UnityEngine;
using System.Collections;

public class LuaUserTypeTest : LuaUnity
{
    MyInstance myInstance;
    MyStaticWrap myStaticWrap;

    public override void SetData()
    {
        base.SetData();

        myInstance = new MyInstance();
        myInstance.Name = "Instance007";
        _lua["myInstance"] = myInstance;

        myStaticWrap = new MyStaticWrap();
        _lua["myStaticWrap"] = myStaticWrap;
    }
}

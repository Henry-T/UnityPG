using UnityEngine;
using System.Collections;

public class LuaEnumTypeTest : LuaUnity {
    EnumReceiver enumReceiver;

    public override void SetData()
    {
        base.SetData();

        enumReceiver = new EnumReceiver();
        _lua["enumReceiver"] = enumReceiver;
    }
}

public enum ETest
{
    One,
    Two,
    Three,
}

public class EnumReceiver
{
    public void LogEnum(ETest e)
    {
        Debug.Log(e.ToString());
    }
}
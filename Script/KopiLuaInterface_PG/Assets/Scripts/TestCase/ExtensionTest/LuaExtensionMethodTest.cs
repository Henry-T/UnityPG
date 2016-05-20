using UnityEngine;
using System.Collections;

public class LuaExtensionMethodTest : LuaUnity {
    EmptyClass emptyClassInstance;

    public override void SetData()
    {
        base.SetData();

        emptyClassInstance = new EmptyClass();
        _lua["emptyClassInstance"] = emptyClassInstance;
    }
}

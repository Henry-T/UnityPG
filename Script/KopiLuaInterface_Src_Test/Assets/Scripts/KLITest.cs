using UnityEngine;
using System.Collections;
using LuaInterface;

public class KLITest : MonoBehaviour {
	
	bool virgin;
	Lua L;
	
	void Start()
	{
		gameObject.renderer.material.color = new Color(1.0f, 0.5f, 0.5f);
		
		virgin = true;
	}
	
	void Update()
	{
		if (virgin)
		{
			virgin = false;
			
			L = new Lua();
			
			gameObject.renderer.material.color = new Color(1.0f, 0.8f, 0.2f);
		
			L.DoString("r = 0");
			L.DoString("g = 1");
			L.DoString("b = 0.2");
			
			float r = (float)(double)L["r"];
			float g = (float)(double)L["g"];
			float b = (float)(double)L["b"];
			
			gameObject.renderer.material.color = new Color(r, g, b);
		
			Debug.Log("1");
			
			L["go"] = gameObject;
			
			Debug.Log("2");
			
			Vector3 v = gameObject.transform.position;
			v.y += 1.0f;
			L["v"] = v;
			
			Debug.Log("3");
			
			L.DoString("go.transform.position = v");
		
			Debug.Log("4");
			
			string[] script = {
				"UnityEngine = luanet.UnityEngine",
				"cube = UnityEngine.GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube)",
				""
			};
			
			foreach (var line in script)
			{
				L.DoString(line);
			}
		}
		
		L.DoString("t = UnityEngine.Time.realtimeSinceStartup");
		L.DoString("q = UnityEngine.Quaternion.AngleAxis(t*50, UnityEngine.Vector3.up)");
		L.DoString("cube.transform.rotation = q");
	}
}

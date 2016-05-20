using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(OrcEquip))]
public class OrcEquipEditor : Editor
{
	public override void OnInspectorGUI(){
		DrawDefaultInspector ();

		OrcEquip oe = (OrcEquip)target;
		if(GUILayout.Button("Equip Axe")){
			oe.EquipAxe();
		}
		if(GUILayout.Button ("Equip Bow")){
			oe.EquipBow();
		}
	}
}


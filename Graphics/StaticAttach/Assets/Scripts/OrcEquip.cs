using UnityEngine;
using System.Collections;

public class OrcEquip : MonoBehaviour {
	public GameObject WeaponAttachPoint;

	void Start () {
	}

	void Update () {
	
	}

	public void UnequipWeapon(){
		foreach (Transform t in WeaponAttachPoint.transform) {
			Destroy(t.gameObject);
		}
	}

	public void EquipBow(){
		UnequipWeapon ();
		GameObject bow = Instantiate (Resources.Load ("Bow"), Vector3.zero, Quaternion.identity) as GameObject;
		
		bow.transform.parent = WeaponAttachPoint.transform;
		bow.transform.localPosition = Vector3.zero;
		bow.transform.localRotation = Quaternion.identity;
		bow.transform.localScale = Vector3.one;
	}

	public void EquipAxe(){
		UnequipWeapon ();
		GameObject axe = Instantiate (Resources.Load ("Axe"), Vector3.zero, Quaternion.identity) as GameObject;
		
		axe.transform.parent = WeaponAttachPoint.transform;
		axe.transform.localPosition = Vector3.zero;
		axe.transform.localRotation = Quaternion.identity;
		axe.transform.localScale = Vector3.one;
	}
}

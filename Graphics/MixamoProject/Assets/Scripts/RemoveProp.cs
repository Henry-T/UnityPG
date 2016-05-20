using UnityEngine;
using System.Collections;

public class RemoveProp : MonoBehaviour {
public string propName;
public Transform targetBone;
public bool  destroyTrigger = true;

void  OnTriggerEnter ( Collider collision  ){
    
    if (targetBone.IsChildOf(collision.transform)){
     	foreach(Transform child in targetBone){
      		if (child.name == propName)
          	Destroy (child.gameObject);    	
  	 }
     
    }
}
}
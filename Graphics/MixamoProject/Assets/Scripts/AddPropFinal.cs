using UnityEngine;
using System.Collections;

public class AddPropFinal : MonoBehaviour {
    public GameObject prop;
    public string propName;
    public Transform targetBone;
    public Vector3 propOffset;
    public bool  destroyTrigger = true;

    void  OnTriggerEnter ( Collider collision  ){
    
        if (targetBone.IsChildOf(collision.transform)){
    	    bool  checkProp = false;
     	    foreach(Transform child in targetBone){
      		if (child.name == propName)
          	    checkProp = true;    	
  	     	}
     
         	if(!checkProp){  
	     	    GameObject newprop;
	     	    newprop = Instantiate(prop, targetBone.position, targetBone.rotation) as GameObject;
	     	    newprop.name = propName;
	     	    newprop.transform.parent = targetBone;
	     	    newprop.transform.localPosition += propOffset;     
	     	    if(destroyTrigger)
	     		    Destroy(gameObject);
    	    }
        }
    }
}
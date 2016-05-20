using UnityEngine;
using System.Collections;

public class LandmineFinal : MonoBehaviour {
	public float range = 50.0f;
	public float force = 2000.0f;
	
	void  OnTriggerEnter ( Collider collision  ){
		if(collision.gameObject.tag == "Player"){
			collision.GetComponent<RagdollCharacterFinal>().ActivateRagdoll();
			Vector3 explosionPos = transform.position;
       	 	Collider[] colliders = Physics.OverlapSphere(explosionPos, range);
        	foreach (Collider hit in colliders) {
				if (hit.rigidbody)
                	hit.rigidbody.AddExplosionForce(force, explosionPos, range, 3.0F);
           		}
			}
	}
}

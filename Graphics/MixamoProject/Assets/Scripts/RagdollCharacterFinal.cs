using UnityEngine;
using System.Collections;

public class RagdollCharacterFinal : MonoBehaviour {	
    private float hitTime;
    private bool wasHit = false;
	void Start () {
	    DeactivateRagdoll();
    }
	void Update () {
	    if(wasHit){
		    if(Time.time >= hitTime + 5.0f)
			    DeactivateRagdoll();
        }
    }
    public void ActivateRagdoll(){
	    this.GetComponent<BasicController>().enabled = false;
	    this.GetComponent<Animator>().enabled = false;
        foreach(Rigidbody bone in GetComponentsInChildren<Rigidbody>()){
		    bone.isKinematic = false;
       	    bone.detectCollisions = true;
	    }
	    wasHit = true;
	    hitTime = Time.time;
    }
	public void DeactivateRagdoll(){
	    this.GetComponent<BasicController>().enabled = true;
	    this.GetComponent<Animator>().enabled = true;
        foreach(Rigidbody bone in GetComponentsInChildren<Rigidbody>()){
		    bone.isKinematic = true;
  		    bone.detectCollisions = false;
	    }
		transform.position = GameObject.Find("Spawnpoint").transform.position;
	    transform.rotation = GameObject.Find("Spawnpoint").transform.rotation;
	    wasHit = false;
    }
}
using UnityEngine;
using System.Collections;

public class ThrowObjectFinal : MonoBehaviour {
	public GameObject projectile;
	public Vector3 projectileOffset;
	public Vector3 projectileForce;
	public Transform charactersHand;
	public float lenghtPrepare;
	public float lenghtThrow;
	public float compensationYAngle = 20.0f;
	private bool prepared = false;
	private bool threw = false;
	private Animator animator;
	
	public void Start(){
	    animator = 	GetComponent<Animator>();
	}
	
    public void LateUpdate(){
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1); 
        if(stateInfo.IsName("UpperBody.Grenade")){
            if(stateInfo.normalizedTime >= lenghtPrepare * 0.01 && !prepared)
				Prepare();
			if(stateInfo.normalizedTime >= lenghtThrow * 0.01 && !threw)
				Throw();
		} else {
	        prepared = false;
	        threw = false;			
	    }
    }
	
	public void Prepare () {
		prepared = true;
		projectile = Instantiate(projectile, charactersHand.position, charactersHand.rotation) as GameObject;
		if(projectile.GetComponent<Rigidbody>())
			Destroy(projectile.rigidbody);
		projectile.GetComponent<SphereCollider>().enabled = false;		
		projectile.name = "projectile";
		projectile.transform.parent = charactersHand;
		projectile.transform.localPosition = projectileOffset;
		projectile.transform.localEulerAngles = Vector3.zero;		
	}	
	public void Throw () {
		threw = true;
		Vector3 dir = transform.rotation.eulerAngles;
		dir.y += compensationYAngle;
		projectile.transform.rotation = Quaternion.Euler(dir);
		projectile.transform.parent = null;		
		projectile.GetComponent<SphereCollider>().enabled = true;		
		projectile.AddComponent<Rigidbody>();
		Physics.IgnoreCollision(projectile.collider, collider);
		projectile.rigidbody.AddRelativeForce(projectileForce);	
	}
}

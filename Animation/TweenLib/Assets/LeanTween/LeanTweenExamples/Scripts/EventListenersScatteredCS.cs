using UnityEngine;
using System.Collections;

public class EventListenersScatteredCS : MonoBehaviour {

	Vector3 towardsRotation;
	float turnForLength = 0.5f;
	float turnForIter = 0f;
	Vector3 fromColor;

	// It's best to make this a public enum that you use throughout your project, so every class can have access to it
	public enum MyEvents{ 
		CHANGE_COLOR,
		JUMP,
		LENGTH
	}

	void Awake(){
		LeanTween.LISTENERS_MAX = 100; // This is the maximum of event listeners you will have added as listeners
		LeanTween.EVENTS_MAX = (int)MyEvents.LENGTH; // The maximum amount of events you will be dispatching

		fromColor = new Vector3(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b);
	}

	void Start () {
		// Adding Listeners, it's best to use an enum so your listeners are more descriptive but you could use an int like 0,1,2,...
		LeanTween.addListener(gameObject, (int)MyEvents.CHANGE_COLOR, changeColor);
		LeanTween.addListener(gameObject, (int)MyEvents.JUMP, jumpUp);
	}

	// ****** Event Listening Methods

	void jumpUp( LTEvent e ){
		rigidbody.AddRelativeForce(Vector3.forward * 300f);
	}

	void changeColor( LTEvent e ){
		Transform tran = (Transform)e.data;
		float distance = Vector3.Distance( tran.position, transform.position);
		Vector3 to = new Vector3(Random.Range(0f,1f),0f,Random.Range(0f,1f));
		LeanTween.value( gameObject, updateColor, fromColor, to, 0.8f ).setRepeat(2).setLoopPingPong().setDelay(distance*0.05f);
	}

	void updateColor( Vector3 v ){
		renderer.material.color = new Color( v.x, v.y, v.z );
	}

	// ****** Physics / AI Stuff

	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.layer!=2)
			towardsRotation = new Vector3(0f, 0f, Random.Range(-180, 180));
    }

     void OnCollisionStay(Collision collision) {
     	if(collision.gameObject.layer!=2){
     		turnForIter = 0f;
	    	turnForLength = Random.Range(0.5f, 1.5f);
	    }
     }

	void FixedUpdate(){
		if(turnForIter < turnForLength){
			rigidbody.MoveRotation( rigidbody.rotation * Quaternion.Euler(towardsRotation * Time.deltaTime ) );
			turnForIter += Time.deltaTime;
		}

		rigidbody.AddRelativeForce(Vector3.down * 4.5f);
	}

	// ****** Key and clicking detection

	void OnMouseDown(){
		if(Input.GetKey( KeyCode.J )){ // Are you also pressing the "j" key while clicking
			LeanTween.dispatchEvent((int)MyEvents.JUMP);
		}else{
			LeanTween.dispatchEvent((int)MyEvents.CHANGE_COLOR, transform); // with every dispatched event, you can include an object (retrieve this object with the *.data var in LTEvent)
		}
	}
}

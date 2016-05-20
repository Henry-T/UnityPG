using UnityEngine;
using System.Collections;

public class BasicController03: MonoBehaviour {
    private Animator animator;
    private CharacterController controller;
    public float transitionTime = .25f;
	
    void Start () {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if(animator.layerCount >= 2)
	        animator.SetLayerWeight(1, 1);
    }

    void Update () {
	    float accelerator = 1.0f;	
        if(controller.isGrounded){	
	        if (Input.GetKey (KeyCode.RightShift) ||Input.GetKey (KeyCode.LeftShift) ){
		        accelerator = 2.0f;
	        } else if(Input.GetKey (KeyCode.RightAlt) ||Input.GetKey (KeyCode.LeftAlt) ){
		       accelerator = 1.5f;
	        } else {
		        accelerator = 1.0f;	
	        }
	        float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
	        float xSpeed = h * accelerator;	
	        float zSpeed = v * accelerator;	
            animator.SetFloat("xSpeed", xSpeed, transitionTime, Time.deltaTime);
	        animator.SetFloat("zSpeed", zSpeed, transitionTime, Time.deltaTime);
	        animator.SetFloat("Speed", Mathf.Sqrt(h*h+v*v), transitionTime, Time.deltaTime);
	        //transform.Rotate(Vector3.up * (Time.deltaTime * v * Input.GetAxis("Mouse X") * 90), Space.World);
	     }

        if (Input.GetKeyDown(KeyCode.F))
            animator.SetBool("Grenade", true);
        else
            animator.SetBool("Grenade", false);

        if (Input.GetButtonDown("Fire1"))
            animator.SetBool("Fire", true);
        if (Input.GetButtonUp("Fire1"))
            animator.SetBool("Fire", false);


    }
}

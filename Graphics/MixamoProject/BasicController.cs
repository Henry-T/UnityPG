using UnityEngine;
using System.Collections;

	public class BasicController : MonoBehaviour {
	private Animator animator;
	private CharacterController controller;
	private Vector3 deltaPosition;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
	public float DirectionDampTime = .25f;
	private float jumpPos = 0.0f;
	private float m_VerticalSpeed = 0;
	private float x_vel = 0.0f;
	private float z_vel = 0.0f;
 
	void Start () {
	controller = GetComponent<CharacterController>();
	animator = GetComponent<Animator>();
	if(animator.layerCount >= 2)
			animator.SetLayerWeight(1, 1);
	}
	
	void Update () {
		
	if(controller.isGrounded){	
		
		if (Input.GetKey(KeyCode.Space)) {
			
			animator.SetBool("Jump", true);
			m_VerticalSpeed = jumpSpeed;
          }else{
			animator.SetBool("Jump", false);                
            }
		if (Input.GetKey (KeyCode.RightShift)){
			animator.SetBool("Running", true);
		} else {
			animator.SetBool("Running", false);
		}
		
		float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
			
		animator.SetFloat("Speed", (h*h+v*v));
        animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);
		animator.SetFloat("ZDirection", v);
		
		if(Input.GetKey(KeyCode.Q) && animator.layerCount >= 2){
				animator.SetBool("TurnLeft", true);
				transform.Rotate(Vector3.up * (Time.deltaTime * -45.0f), Space.World);
		}  else {
		animator.SetBool("TurnLeft", false);	
		}
		if(Input.GetKey(KeyCode.E) && animator.layerCount >= 2)
		{
			animator.SetBool("TurnRight", true);
			transform.Rotate(Vector3.up * (Time.deltaTime * 45.0f), Space.World);
				
		}  else {
		animator.SetBool("TurnRight", false);	
		}
		if(Input.GetKeyDown(KeyCode.F) && animator.layerCount >= 2){
				animator.SetBool("Grenade", true);
		} else {
			animator.SetBool("Grenade", false);
		}
		if(Input.GetButtonDown("Fire1") && animator.layerCount >= 2){
				animator.SetBool("Fire", true);
		}
		if(Input.GetButtonUp("Fire1") && animator.layerCount >= 2){
			animator.SetBool("Fire", false);
		}
	


			
		}
		

	
	}
	
	
	void OnAnimatorMove(){
		
		

		Vector3 deltaPosition = animator.deltaPosition;
		
			
		
		//float dir = animator.rootRotation.y;
		//dir = 1.0f;
		//print ("dir: " + dir + "rot: " + animator.rootRotation.y);
		
		if(controller.isGrounded){
		x_vel = animator.GetFloat("Speed")  * controller.velocity.x * 0.25f;
		z_vel = animator.GetFloat("Speed") * controller.velocity.z * 0.25f;
		print (controller.velocity);
		}
		
		m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;	
		//print (m_VerticalSpeed);
		
	
		
		if(m_VerticalSpeed <= 0){
			animator.SetBool("Jump", false);  
		}
				
		deltaPosition.y = m_VerticalSpeed * Time.deltaTime;
				
		
		if(!controller.isGrounded){
	    deltaPosition.x = x_vel * Time.deltaTime;
		deltaPosition.z = z_vel * Time.deltaTime;
		
			
			
		}


			//if (controller.Move(deltaPosition) == CollisionFlags.Below) m_VerticalSpeed = 0;			
		//if (controller.Move(deltaPosition) == CollisionFlags.Below){
		//m_VerticalSpeed = 0;
		//} else {
			
		//}
		controller.Move(deltaPosition);
		if ((controller.collisionFlags & CollisionFlags.Below) != 0){
		m_VerticalSpeed = 0;	
		
		}
		transform.rotation = animator.rootRotation;
	}
	}

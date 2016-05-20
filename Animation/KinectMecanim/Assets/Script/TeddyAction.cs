using UnityEngine;
using System.Collections;

public class TeddyAction : MonoBehaviour {

	private Animator m_animator;
	private AnimatorStateInfo stateInfo;
	private AnimatorStateInfo stateInfo02;

	private float horizontal = 0.0f;
	private float vertical = 0.0f;

	// Use this for initialization
	void Start () {
	
		m_animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (m_animator) {

			stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
			stateInfo02 = m_animator.GetCurrentAnimatorStateInfo(1);
			
			if (stateInfo.IsName("Base Layer.Run")) {

				if (Input.GetKeyDown(KeyCode.Space)) {

					m_animator.SetBool("isJump", true);
				}

			}
			else {

				m_animator.SetBool("isJump", false);
			}

			if (stateInfo02.IsName("Wave Layer.DoNothing")) {

				if (Input.GetKeyDown(KeyCode.T)) {

					m_animator.SetBool("sayHi", true);
				}
			}
			else {

				m_animator.SetBool("sayHi", false);
			}

			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");

			m_animator.SetFloat("speed", horizontal*horizontal+vertical*vertical);
			m_animator.SetFloat("direction", horizontal);
		}
	}
}

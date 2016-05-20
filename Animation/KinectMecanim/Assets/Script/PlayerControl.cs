using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	public NetworkPlayer ownerPlayer;
	private Animator m_animator;
	private AnimatorStateInfo m_stateInfo;

	//使用public是为了测试观看数据比较直接
	public float clientHInput = 0;
	public float clientVInput = 0;

	public float serverHInput = 0;
	public float serverVInput = 0;

	public float currentSpeed = 0;
	public float currentDirection = 0;

	void Awake() {
		//首先拿到animator的引用
		m_animator = gameObject.GetComponent<Animator>();

		if (Network.isClient) {

			enabled = false;
		}
	}

	void Update () {
	
		if (ownerPlayer != null && Network.player == ownerPlayer) {
			//未使用，可删除
			m_stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);

			float currentHInput = Input.GetAxis("Horizontal");
			float currentVInput = Input.GetAxis("Vertical");

			if (clientHInput != currentHInput || clientVInput != currentVInput) {

				clientHInput = currentHInput;
				clientVInput = currentVInput;

				if (Network.isServer) {

					SendMovementInput(currentHInput, currentVInput);
				}
				else if (Network.isClient) {
					networkView.RPC("SendMovementInput", RPCMode.Server, currentHInput, currentVInput);
				} 
			}
		}

		if (Network.isServer) {

			currentSpeed = serverHInput*serverHInput+serverVInput*serverVInput;
			currentDirection = serverHInput;
		}

		m_animator.SetFloat("speed", currentSpeed);
		m_animator.SetFloat("direction", currentDirection);
	}

	[RPC]
	void SetPlayer(NetworkPlayer player) {

		ownerPlayer = player;

		if (player == Network.player) {

			enabled = true;
		}
	}

	[RPC]
	void SendMovementInput(float currentHInput, float currentVInput) {

		serverHInput = currentHInput;
		serverVInput = currentVInput;
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {

			float speed = currentSpeed;
			float direction = currentDirection;

			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;

			stream.Serialize(ref speed);
			stream.Serialize(ref direction);

			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
		} else {

			float speed = 0;
			float direction = 0;
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;

			stream.Serialize(ref speed);
			stream.Serialize(ref direction);

			stream.Serialize(ref pos);
			stream.Serialize(ref rot);

			currentSpeed = speed;
			currentDirection = direction;

			m_animator.SetFloat("speed", currentSpeed);
			m_animator.SetFloat("direction", currentDirection);

			transform.position = pos;
			transform.rotation = rot;
		}
	}
}

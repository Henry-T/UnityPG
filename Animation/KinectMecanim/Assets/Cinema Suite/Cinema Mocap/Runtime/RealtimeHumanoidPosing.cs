using UnityEngine;

[ExecuteInEditMode]
public class RealtimeHumanoidPosing : MonoBehaviour
{
    private GameObject CHARACTER;
    private GameObject HIP;
    private GameObject SPINE;
    private GameObject SHOULDER_CENTER;
    private GameObject HEAD;
    private GameObject SHOULDER_LEFT;
    private GameObject ELBOW_LEFT;
    private GameObject WRIST_LEFT;
    private GameObject HAND_LEFT;
    private GameObject SHOULDER_RIGHT;
    private GameObject ELBOW_RIGHT;
    private GameObject WRIST_RIGHT;
    private GameObject HAND_RIGHT;
    private GameObject HIP_LEFT;
    private GameObject KNEE_LEFT;
    private GameObject ANKLE_LEFT;
    private GameObject FOOT_LEFT;
    private GameObject HIP_RIGHT;
    private GameObject KNEE_RIGHT;
    private GameObject ANKLE_RIGHT;
    private GameObject FOOT_RIGHT;

    private GameObject[] joints;

    private Vector3 startingPosition;

    public void Awake()
    {
        CHARACTER = this.gameObject;

        if (CHARACTER != null)
        {
			HIP = CHARACTER.transform.FindChild("Bip01").gameObject;
			Transform _spine = HIP.transform.FindChild("Bip01 Pelvis").FindChild("Bip01 Spine");

			HIP_LEFT = _spine.transform.FindChild("Bip01 L Thigh").gameObject;
			KNEE_LEFT = HIP_LEFT.transform.FindChild("Bip01 L Calf").gameObject;
			ANKLE_LEFT = KNEE_LEFT.transform.FindChild("Bip01 L Foot").gameObject;
			FOOT_LEFT = ANKLE_LEFT.transform.FindChild("Bip01 L Toe0").gameObject;

			HIP_RIGHT = _spine.transform.FindChild("Bip01 R Thigh").gameObject;
			KNEE_RIGHT = HIP_RIGHT.transform.FindChild("Bip01 R Calf").gameObject;
			ANKLE_RIGHT = KNEE_RIGHT.transform.FindChild("Bip01 R Foot").gameObject;
			FOOT_RIGHT = ANKLE_RIGHT.transform.FindChild("Bip01 R Toe0").gameObject;

			SPINE = _spine.transform.FindChild("Bip01 Spine1").gameObject;

			SHOULDER_CENTER = SPINE.transform.FindChild("Bip01 Spine2").FindChild("Bip01 Neck").gameObject;
			HEAD = SHOULDER_CENTER.transform.FindChild("Bip01 Head").gameObject;

			SHOULDER_LEFT = SHOULDER_CENTER.transform.FindChild("Bip01 L Clavicle").FindChild("Bip01 L UpperArm").gameObject;
			ELBOW_LEFT = SHOULDER_LEFT.transform.FindChild("Bip01 L Forearm").gameObject;
			WRIST_LEFT = ELBOW_LEFT.transform.FindChild("Bip01 L Hand").gameObject;
			HAND_LEFT = WRIST_LEFT.transform.FindChild("Bip01 L Finger1").gameObject;

			SHOULDER_RIGHT = SHOULDER_CENTER.transform.FindChild("Bip01 R Clavicle").FindChild("Bip01 R UpperArm").gameObject;
			ELBOW_RIGHT = SHOULDER_RIGHT.transform.FindChild("Bip01 R Forearm").gameObject;
			WRIST_RIGHT = ELBOW_RIGHT.transform.FindChild("Bip01 R Hand").gameObject;
			HAND_RIGHT = WRIST_RIGHT.transform.FindChild("Bip01 R Finger1").gameObject;
        }

        joints = new GameObject[25] {
			null, HIP, SPINE, SHOULDER_CENTER,
			null, SHOULDER_LEFT, ELBOW_LEFT, WRIST_LEFT,
			null, SHOULDER_RIGHT, ELBOW_RIGHT, WRIST_RIGHT,
			HIP, HIP_LEFT, KNEE_LEFT, ANKLE_LEFT,
			null, HIP_RIGHT, KNEE_RIGHT, ANKLE_RIGHT,
			HEAD, HAND_LEFT, HAND_RIGHT, FOOT_LEFT, FOOT_RIGHT};
    }

    public void SetWorldPosition(Vector3 position)
    {
        if (startingPosition == Vector3.zero)
        {
            startingPosition = position - CHARACTER.transform.position;
        }
        CHARACTER.transform.position = position - startingPosition;
    }

    public void SetRotations(Quaternion[] rotations)
    {
        for (int i = 0; i < rotations.Length; i++)
        {
            if (joints[i] != null)
            {
                joints[i].transform.localRotation = rotations[i];
            }
        }
    }
}


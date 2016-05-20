using UnityEngine;
using System.Collections;

public class AnyStateSwitchTest : MonoBehaviour {

    public Animator Animator;
    private bool changedFlag = false;

	void Start () {
        AnimControlTest.hashOfIdle = Animator.StringToHash(AnimControlTest.nameOfIdle);
        AnimControlTest.hashOfFire = Animator.StringToHash(AnimControlTest.nameOfFire);
	}
	
	void Update () {
        if(changedFlag)
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).nameHash == AnimControlTest.hashOfFire)
                Debug.Log("那么..在随后一次Update时：它确实是延迟一帧更新了");
            else
                Debug.Log("那么..在随后一次Update时：它居然过了一帧还不更新！坑的尺寸变大了！");

            changedFlag = false;
        }
        //Animator.SetBool("anyFire", false);
        //Animator.SetBool("anyIdle", false);
        //Animator.SetBool("anyRun", false);
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0, 100, 40), new GUIContent("Fire!")))
        {
            Animator.SetTrigger("anyFire");
            changedFlag = true;
        }
    }
}

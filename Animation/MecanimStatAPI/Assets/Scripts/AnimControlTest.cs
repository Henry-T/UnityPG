using UnityEngine;
using System.Collections;

public class AnimControlTest : MonoBehaviour {

    public Animator Animator;
    private bool changedFlag = false;

    public static string nameOfIdle = "Base Layer.Idle";
    public static string nameOfFire = "Base Layer.Fire";
    public static int hashOfIdle = 0;
    public static int hashOfFire = 0;

    bool timer10Flag = false;
    float timer10;
    bool timer50Flag = false;
    float timer50 = 0;

	void Start () {
        hashOfIdle = Animator.StringToHash(nameOfIdle);
        hashOfFire = Animator.StringToHash(nameOfFire);

        Debug.Log("提前声明：" + nameOfIdle + "的Hash值为" + hashOfIdle);
        Debug.Log("提前声明：" + nameOfFire + "的Hash值为" + hashOfFire);
	}
	
	void Update () {
        if (changedFlag)
        {
            changedFlag = false;
            if(Animator.GetCurrentAnimatorStateInfo(0).nameHash == hashOfFire)
                Debug.Log("那么..在随后一次Update时：它确实是延迟一帧更新了");
            else
                Debug.Log("那么..在随后一次Update时：它居然过了一帧还不更新！坑的尺寸变大了！");
        }

        if(timer10Flag)
        {
            timer10 += Time.deltaTime;
            if (timer10 > 0.01)
            {
                timer10Flag = false;
                if (Animator.GetCurrentAnimatorStateInfo(0).nameHash == hashOfFire)
                    Debug.Log("那么..在随后的10ms后：它最后确实是更新了");
                else
                    Debug.Log("那么..在随后的10ms后：它还是没有更新！");
            }
        }

        if(timer50Flag)
        {
            timer50 += Time.deltaTime;
            if (timer50 > 0.05)
            {
                timer50Flag = false;
                if (Animator.GetCurrentAnimatorStateInfo(0).nameHash == hashOfFire)
                    Debug.Log("那么..在随后的50ms后：它最后确实是更新了");
                else
                    Debug.Log("那么..在随后的50ms后：它还是没有更新！");
            }
        }
	}

    void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,40), new GUIContent("Fire-轻点,易坏")))
        {
            int oldHash = Animator.GetCurrentAnimatorStateInfo(0).nameHash;
            Animator.Play("Base Layer.Fire");
            int instantHash = Animator.GetCurrentAnimatorStateInfo(0).nameHash;

            if (instantHash == hashOfFire)
                Debug.Log("以nameHash接口检查: 立即更新");
            else
                Debug.Log("以nameHash接口检查: 延迟更新");

            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(nameOfFire))
                Debug.Log("以isName接口检查: 立即更新");
            else
                Debug.Log("以isName接口检查: 延迟更新");

            if (instantHash == 1432961145)
                Debug.Log("硬比对下当前hash和"+nameOfIdle+"的hash，果然是一样的。大坑！动画更新真的被延迟了！");
            if (oldHash == instantHash)
                Debug.Log("对比调用Play前后两次取得的nameHash，是一样的");

            changedFlag = true;
            timer10Flag = true;
            timer50Flag = true;
            timer10 = 0;
            timer50 = 0;
        }
    }
}

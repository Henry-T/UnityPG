using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;

public class GetStateListTest : MonoBehaviour {

    public Animator Animator;

    void OnGUI()
    {
        if(GUI.Button(new Rect(0, 100,100, 40), new GUIContent("获取State")))
        {
            UnityEditorInternal.AnimatorController animController = Animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;

            int layerCount = animController.layerCount;
            Debug.Log(string.Format("动画层数量: {0}", layerCount));

            // 动画层名称
            for (int layer = 0; layer < layerCount; layer++)
            {
                Debug.Log(string.Format("Layer {0}: {1}", layer, animController.GetLayer(layer).name));
            }

            // 动画层0上面的State
            UnityEditorInternal.StateMachine sm = animController.GetLayer(0).stateMachine;
            for(int i = 0; i < sm.stateCount; i++)
            {
                UnityEditorInternal.State state = sm.GetState(i);
                Debug.Log(string.Format("State: {0}, 唯一名称: {1}", state.name, state.uniqueName));
            }
        }
    }
}

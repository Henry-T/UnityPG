using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ViewPipelineDebugger : MonoBehaviour {

    public static ViewPipelineDebugger Instance { get { return instance; } }
    private static ViewPipelineDebugger instance;

    public Transform Root;   

    void Start()
    {
        instance = this;
    }

    public void Refresh()
    {
        foreach (Transform child in Root) {
            GameObject.Destroy(child.gameObject);
        }

        int layer = 1;
        int stack = 0;

        var stateInfoAry = ViewPipeline.Instance.stateInfoStack.ToArray();

        for (int i=0; i<stateInfoAry.Length; i++)
        {
            var stateInfo = stateInfoAry[i];
            stack = stack + 1;
            if (stateInfo.Mode == ViewStateInfo.EMode.Layer)
            {
                if (i != 0)
                {
                    layer += 1;
                }
                stack = 1;
                createNode(stateInfo, layer, stack);
            }
            else
            {
                createNode(stateInfo, layer, stack);
            }
        }
    }

    private void createNode(ViewStateInfo viewStateInfo, int layer, int stack)
    {
        GameObject node = GameObject.Instantiate(Resources.Load<GameObject>("Node")) as GameObject;
        if (viewStateInfo.Instance != null)
        {
            node.GetComponent<Image>().material = Resources.Load<Material>("matNode");
        }
        else
        {
            node.GetComponent<Image>().material = Resources.Load<Material>("matNodeGray");
        }

        node.GetComponentInChildren<Text>().text = viewStateInfo.Type.Name;
        node.transform.SetParent(Root);
        node.transform.position = new Vector3(10 + layer * 90, 28 * stack);
    }
}

using UnityEngine;
using System.Collections;

public class IllusionControl : MonoBehaviour {

	private Renderer[] renderers;
	private float curTime = 0;
	public float FadeTime = 0.5f;
	public bool AutoDestroy = true;

	// Use this for initialization
	void Start () {
		renderers = GetComponentsInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		curTime += Time.deltaTime;

		if(AutoDestroy && curTime > FadeTime)
		{
			Destroy(gameObject);
		}
		else
		{
			Color from = new Color(0,0,0,1);
			Color to = new Color(0,0,0,0);
			Color now = Color.Lerp(from, to, curTime/FadeTime);

			foreach(Renderer renderer in renderers)
			{
				if(renderer)
				{
					foreach(Material mat in renderer.materials)
					{
						mat.color = now;
					}
				}
			}
		}
	}
}

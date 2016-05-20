using UnityEngine;
using System.Collections;

// WRAN !!! 此文件必须在客户端和Resource中同步
public class AnimCutout : MonoBehaviour {
	private Renderer renderer;

	public float StartValue = 1f;
	public float EndValue = 0f;
	public float Duration = 1f;
	public float Delay = 0f;
	private float timer = 0;
	private float curValue = 0;
	private bool animStarted = false;
	private bool animFinished = false;
	public string ShaderParam = "_Cutoff";

	// Use this for initialization
	void Start () {
		renderer = gameObject.renderer;
		timer = 0;
		setValue(StartValue);
	}
	
	// Update is called once per frame
	void Update () {
		if(!renderer || Duration <= 0)
			return;

		if(!animStarted)
		{
			timer += Time.deltaTime;
			if(timer >= Delay)
			{
				animStarted = true;
				timer = 0;
			}
		}
		else if(!animFinished)
		{
			timer += Time.deltaTime;
			if(timer <= Duration)
			{
				curValue = Mathf.Lerp(StartValue, EndValue, timer/Duration);
				setValue(curValue);
			}
			else
			{
				animFinished = true;
				timer = 0;
				setValue(EndValue);
			}
		}
		else
		{

		}
	}

	void setValue(float value)
	{
		if(renderer && renderer.material)
			renderer.material.SetFloat(ShaderParam, value);
	}
}

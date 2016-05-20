using UnityEngine;
using System.Collections;

public class EnemyComp : MonoBehaviour {

	public static float TO_SHOW_TIME = 0.2f;
	public static float SHOW_TIME = 0.4f;
	public static float TO_HIDE_TIME = 0.2f;
	public static float HIDE_TIME = 0.4f;

	public enum EEnemyState
	{
		Show,
		ShowToHide,
		Hide,
		HideToShow,
	}

	public float ShareTimer = 0;
	public EEnemyState State;

	private Renderer renderer;

	public int CurSlot = -1;

	// Use this for initialization
	void Start () {
		renderer = GetComponentInChildren<Renderer>();
	}

	public void Restart()
	{
		State = EEnemyState.Hide;
		ShareTimer = 0;
		gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		if(!GameManager.Instance.IsFighting)
			gameObject.SetActive(false);

		ShareTimer += Time.deltaTime;

		if(State == EEnemyState.Hide)
		{
			if(ShareTimer > HIDE_TIME)
			{
				int id = Random.Range(0, GameManager.Instance.VillageComp.BuildingGroupComp.BuildingCompList.Count - 1);
				BuildingComp buildingComp = GameManager.Instance.VillageComp.BuildingGroupComp.BuildingCompList[id];

				if(buildingComp)
				{
					transform.position = buildingComp.transform.position + Vector3.up * 2;

					State = EEnemyState.HideToShow;
					ShareTimer = 0;
					CurSlot = buildingComp.Data.SlotID;

					//animation["HideToShow"].normalizedSpeed = TO_SHOW_TIME;
					//animation.Play("HideToShow");
				}
			}
		}
		else if(State == EEnemyState.HideToShow)
		{
			if(ShareTimer > TO_SHOW_TIME)
			{
				State = EEnemyState.Show;
				ShareTimer = 0;
			}
			else
			{
				renderer.material.color = new Color(1,1,1, ShareTimer / TO_SHOW_TIME);
			}
		}
		else if(State == EEnemyState.Show)
		{
			if(ShareTimer > SHOW_TIME)
			{
				State = EEnemyState.ShowToHide;
				ShareTimer = 0;

				//animation["HideToShow"].normalizedSpeed = TO_HIDE_TIME;
				//animation.Play("ShowToHide");
			}
		}
		else if(State == EEnemyState.ShowToHide)
		{
			if(ShareTimer > TO_HIDE_TIME)
			{
				State = EEnemyState.Hide;
				ShareTimer = 0;
			}
			else
			{
				renderer.material.color = new Color(1,1,1, 1-ShareTimer / TO_SHOW_TIME);
			}
		}
	}
}

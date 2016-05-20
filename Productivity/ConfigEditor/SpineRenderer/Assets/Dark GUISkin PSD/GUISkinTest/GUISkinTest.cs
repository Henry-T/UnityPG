using UnityEngine;
using System.Collections;

public class GUISkinTest : MonoBehaviour
{
	public GUISkin[] thisGUISkins;
	
	private bool error_GUISkins = false;
	private int selectedGUISkin = 0;
	
    private Rect rctWindow1;
    private Rect rctWindow2;
    private bool blnToggleState = true;
    private float fltSliderValue = 0.5f;
    private Vector2 scrollPosition = Vector2.zero;
	
    void Awake()
    {
		if (thisGUISkins.Length <= 0) {
			Debug.LogError("Missing GUI Skin, assign a GUI Skins in the inspector");
			error_GUISkins = true;
		}
		else {
			for (int s = 0; s < thisGUISkins.Length; s++) {
				if (!thisGUISkins[s]) {
					Debug.LogError("Missing GUI Skin #" + s + ", assign a GUI Skin in the inspector");
					error_GUISkins = true;
				}
			}
		}
		
		rctWindow1 = new Rect(20, 20, 200, 100);
		rctWindow2 = new Rect(240, 20, 200, 380);
    }
	
    void OnGUI()
    {
		if (error_GUISkins == false) {
			GUI.skin = thisGUISkins[selectedGUISkin];
			rctWindow1 = GUILayout.Window(0, rctWindow1, DoConfigWindow, "GUI Skin Config Window", GUI.skin.GetStyle("window"));
			rctWindow2 = GUILayout.Window(1, rctWindow2, DoMyWindow, thisGUISkins[selectedGUISkin].name, GUI.skin.GetStyle("window"));
		}
    }

    void DoConfigWindow(int windowID)
    {
		GUILayout.BeginVertical();
		GUILayout.Label("Select GUI Skin:");
		GUILayout.Space(2);
		for (int t = 0; t < thisGUISkins.Length; t++) {
			GUILayout.BeginHorizontal();
			string button_value;
			if (t == selectedGUISkin) {
				button_value = "--- " + thisGUISkins[t].name + " ---";
			}
			else {
				button_value = thisGUISkins[t].name;
			}
			if (GUILayout.Button(button_value)) {
				selectedGUISkin = t;
				
				rctWindow1 = new Rect(rctWindow1.x, rctWindow1.y, 200, 100);
				rctWindow2 = new Rect(rctWindow2.x, rctWindow2.y, 200, 380);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
        GUI.DragWindow();
    }
	
    void DoMyWindow(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Im a Label");
		GUILayout.Box("Im a Box\nIm the second line");
		GUILayout.Space(4);
        GUILayout.Button("Im a Button");
		GUILayout.Button("Im a ButtonIcon", "ButtonIcon");
        GUILayout.TextField("Im a Text Field");
        GUILayout.TextArea("Im a Text Area\nIm the second line\nIm the third line");
        blnToggleState = GUILayout.Toggle(blnToggleState, "Im a Toggle");
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(8);
        //Sliders
        GUILayout.BeginHorizontal();
        fltSliderValue = GUILayout.HorizontalSlider(fltSliderValue, 0.0f, 1.1f);
        fltSliderValue = GUILayout.VerticalSlider(fltSliderValue, 0.0f, 1.1f, GUILayout.Height(50));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        //Scrollbars
		GUILayout.BeginHorizontal();
		GUILayout.Space(8);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Height(80), GUILayout.Width(156));
        for (int i = 0; i < 8; i++)
        {
            GUILayout.Label("Im the #" + (i + 1) +" very long line of text", GUILayout.Width(205));
        }
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();
		GUILayout.Space(8);
		GUILayout.EndVertical();
        GUI.DragWindow();
    }
}

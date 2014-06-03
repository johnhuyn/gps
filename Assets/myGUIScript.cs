using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class myGUIScript : MonoBehaviour
{
	
	public GUISkin myGUISkin;
	public List<string> debugs;
	private string group_name;
	private string drawing_name;
	public bool penDown = false;
	public int stroke_name;
	private Color penColor;
	ColorPicker[] color_Picker;
	private myNetworkHelperScript network_helper;
	
	// Use this for initialization
	void Start ()
	{
		
		if (this.myGUISkin == null) {
			addDebug ("Please assign a GUIskin on the editor!");
			this.enabled = false;
			return;
		}
		
		network_helper = GameObject.Find ("myNetworkHelper").GetComponentInChildren<myNetworkHelperScript> ();
		
		group_name = "jkw";
		drawing_name = "kappa";
		stroke_name = 0;
		
		color_Picker = GameObject.FindObjectsOfType<ColorPicker> ();
		foreach (ColorPicker elem in color_Picker) {
			elem.useExternalDrawer = true;
		}
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	void OnGUI ()
	{
		GUI.skin = this.myGUISkin;
		group_name = GUI.TextField (new Rect (0, 0, Screen.width, 100), group_name);
		drawing_name = GUI.TextField (new Rect (0, 100, Screen.width, 100), drawing_name);
		
		foreach (ColorPicker cp in color_Picker) {
			cp._DrawGUI ();
		}
		
		bool oldPenDown = penDown;
		penDown = GUI.Toggle (new Rect (0, 300, Screen.width, 100), penDown, "Pen Down");
		
		if (GUI.changed) {
			if (oldPenDown != penDown) {
				if (penDown) {
					stroke_name++;
					network_helper.addStrokeColor ("" + stroke_name, penColor);
					addDebug ("Pen on, Stroke name: " + stroke_name + ", Color: " + penColor);
					oldPenDown = penDown;
				} else {
					addDebug ("Pen off.");
				}
			}
		}
		
		
		if (GUI.Button (new Rect (0, 400, Screen.width, 100), "Upload")) {
			addDebug ("Uploading data with group name (" + group_name + ") and drawing name (" + drawing_name + ")");
			network_helper.addStrokeColor ("Stroke Name", penColor);
			network_helper.uploadPoints(group_name, drawing_name);
			
		}
		int temp = 500;
		foreach (string d in debugs) {
			GUI.Label (new Rect (0, temp, Screen.width, 100), d);
			temp += 100;
		}
		while (debugs.Count > 2) {
			debugs.RemoveAt (0);
		}
	}
	
	void OnSetColor(Color color)
	{
		penColor = color;
		addDebug ("New Color " + color);
	}
	
	void OnGetColor (ColorPicker picker)
	{
		picker.NotifyColor (penColor);
	}
	
	public void addDebug (string e)
	{
		debugs.Add (e);
		Debug.Log (e);
	}
	
}

using UnityEngine;
using System.Collections;

public class myLocationScript : MonoBehaviour
{
	
	public GUISkin myGUISkin;
	private myNetworkHelperScript network_helper;
	private myGUIScript local_gui;
	private bool working = false;
	
	// Use this for initialization
	IEnumerator Start ()
	{
		local_gui = GameObject.Find ("myGUI").GetComponentInChildren<myGUIScript> ();
		network_helper = GameObject.Find ("myNetworkHelper").GetComponentInChildren<myNetworkHelperScript> ();
		
		if (this.myGUISkin == null) {  
			local_gui.addDebug ("Please assign a GUIskin on the editor!");  
			this.enabled = false;  
			yield return false;  
		} 
		
		if (!Input.location.isEnabledByUser) {
			local_gui.addDebug ("User has not enabled Location");
			yield return false;
		}
		
		Input.location.Start (1f, 1f);
		
		// Wait until service initializes
		int maxWait = 20;
		while ((Input.location.status == LocationServiceStatus.Initializing) && (maxWait > 0)) {
			yield return new WaitForSeconds (1);
			maxWait--;
		}
		
		// Service didn't initialize in 20 seconds
		if (maxWait < 1) {
			print ("Timed out");
			yield return false;
		}
		
		// Connection has failed
		if (Input.location.status == LocationServiceStatus.Failed) {
			local_gui.addDebug ("Unable to determine device location");
			return false;
		} else {
			local_gui.addDebug ("Location: " + Input.location.lastData.latitude + " " +
			                    Input.location.lastData.longitude + " " +
			                    (long)Input.location.lastData.timestamp);
			working = true;
		}
		
		
	}
	
	float lastLongitude = -1;
	float lastLatitude = -1;
	float lastAltitude = -1;
	
	// Update is called once per frame
	void Update ()
	{
		if (working) {
			
			if (lastLongitude != Input.location.lastData.longitude || lastLatitude != Input.location.lastData.latitude || lastAltitude != Input.location.lastData.altitude) {
				lastLongitude = Input.location.lastData.longitude;
				lastLatitude = Input.location.lastData.latitude;
				lastAltitude = Input.location.lastData.altitude;
				
				local_gui.addDebug ("Location Changed: " + lastLatitude + " " + lastLongitude + " " + lastAltitude);
				
			}
			if (local_gui.penDown) {
				//	local_gui.addDebug ("New Location Recorded: " + lastLatitude + " " +lastLongitude + " " + lastAltitude);
				network_helper.addPoint ("" + local_gui.stroke_name, (long)Input.location.lastData.timestamp, lastLatitude, lastLongitude, lastAltitude);
				
			}
		}
	}
	
	void OnGUI ()
	{
		GUI.skin = this.myGUISkin;
		
		GUI.Label (new Rect (0, Screen.height - 150, Screen.width, 40), "Strokes/Points: " + network_helper.numStrokes + "/" + network_helper.numPoints);
		GUI.Label (new Rect (0, Screen.height - 100, Screen.width, 40), "Current Location: " + lastLatitude + " " + lastLongitude + " " + lastAltitude);
		
	}
}

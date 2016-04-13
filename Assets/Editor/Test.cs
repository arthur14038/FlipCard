using UnityEngine;
using System;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(10, 10, 150, 50), "Init"))
		{
			//DateTime now = DateTime.Now;
			//DateTime tmp = new DateTime(2016, 4, 12, 10, 0, 0);
			//TimeSpan span = now - tmp;
			//Debug.LogFormat("Hours: {0}", span.Hours);
			//Debug.LogFormat("TotalHours: {0}", span.TotalHours);

			Debug.Log(DateTime.MinValue.Ticks.ToString());

			//Debug.Log(Random.Range(0, 2));
		}		
	}
}

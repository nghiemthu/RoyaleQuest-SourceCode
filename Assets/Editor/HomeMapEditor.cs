using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (HomeMap))]
public class HomeMapEditor : Editor {

	public override void OnInspectorGUI ()
	{

		HomeMap homeMap = target as HomeMap;

		if (DrawDefaultInspector ()) {
			homeMap.GenerateMap ();
		}

		if (GUILayout.Button("Generate Map")) {
			homeMap.GenerateMap ();
		}


	}

}

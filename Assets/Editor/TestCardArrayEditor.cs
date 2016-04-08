﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TestCardArray))]
public class TestCardArrayEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TestCardArray myTarget = (TestCardArray)target;

		myTarget.cardParent = EditorGUILayout.ObjectField("Card Parent", myTarget.cardParent, typeof(Transform), true) as Transform;

		myTarget.totalCardCount = EditorGUILayout.IntField("Total Card Count", myTarget.totalCardCount);
		myTarget.cardSize = EditorGUILayout.FloatField("Card Size", myTarget.cardSize);

		if(EditorApplication.isPlaying)
			if(GUILayout.Button("Show Cards"))
			{
				myTarget.LoadAndShowCards();
			}
		//EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
	}
}

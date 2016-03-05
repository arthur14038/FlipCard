using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

public class FlipCardEditor : EditorWindow
{
	bool showPlayerMessage = true;
	int unlockMode;

	[MenuItem("Window/FlipCardEditor")]
	static void Init()
	{
		FlipCardEditor window = (FlipCardEditor)EditorWindow.GetWindow(typeof(FlipCardEditor));
		window.autoRepaintOnSceneChange = true;
	}

	void OnGUI()
	{
		if(GUILayout.Button("PlayerMessage"))
		{
			showPlayerMessage = !showPlayerMessage;
		}
		if(showPlayerMessage)
		{
			GUILayout.Label("MusicSetting: " + PlayerPrefsManager.MusicSetting);
			GUILayout.Label("SoundSetting: " + PlayerPrefsManager.SoundSetting);
			GUILayout.Label("UnlockMode: " + PlayerPrefsManager.UnlockMode);
		}

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("設定UnlockMode", GUILayout.Width(250)))
		{
			PlayerPrefsManager.UnlockMode = unlockMode;
		}
		unlockMode = EditorGUILayout.IntField(unlockMode);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("清除PlayerPrefs"))
		{
			PlayerPrefs.DeleteAll();
			DeleteGameRecord();
        }
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("清除Material"))
		{
			ClearAllMaterial(Selection.gameObjects[0]);
		}
		GUILayout.EndHorizontal();
	}

	void DeleteGameRecord()
	{
		string filePath = GetSaveFilePath("GameRecord.json");
		string timeModeRecordPath = GetSaveFilePath("TimeModeRecord.json");
		string classicModeRecordPath = GetSaveFilePath("ClassicModeRecord.json");
		WriteFileTool.DeleteFile(filePath);
		WriteFileTool.DeleteFile(timeModeRecordPath);
		WriteFileTool.DeleteFile(classicModeRecordPath);
	}

	string GetSaveFilePath(string fileName)
	{
		return Application.persistentDataPath + "/" + fileName;
	}

	void ClearAllMaterial(GameObject parent)
	{
		Image[] images = parent.GetComponentsInChildren<Image>(true);

		foreach(Image image in images)
		{
			if(image.material != null)
			{
				if(image.material.name == "SpriteMaterial" || image.material.name == "MaskMaterial")
					image.material = null;
				else
					Debug.Log(image.material.name);
            }
		}
	}
}

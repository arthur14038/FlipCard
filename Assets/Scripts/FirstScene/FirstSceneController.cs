﻿using UnityEngine;
using System.Collections;

public class FirstSceneController : AbstractController {

	public override IEnumerator Init ()
	{
		yield return new WaitForSeconds(0.5f);
		GameMainLoop.Instance.ChangeScene(SceneName.MainScene);
		//GameMainLoop.Instance.ChangeScene(SceneName.TestMain);
		yield return new WaitForSeconds(0.3f);
        this.gameObject.SetActive(false);
    }
}

using UnityEngine;
using System.Collections;

public class FirstSceneController : AbstractController {
	public override IEnumerator Init ()
	{
		yield return new WaitForSeconds(1f);
		GameMainLoop.Instance.ChangeScene(SceneName.MainScene);
        yield return new WaitForSeconds(0.3f);
        this.gameObject.SetActive(false);
    }
}

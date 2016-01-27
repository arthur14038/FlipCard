using UnityEngine;
using System.Collections;

public abstract class AbstractController : MonoBehaviour, IController
{
	public UnityEngine.UI.CanvasScaler[] canvasScaler;
	protected bool NeedExpand
	{
		get
		{
			float x = (float)Screen.width / (float)Screen.height;
			return x > (1200f / 1848f);
		}
	}

	protected virtual void Start()
	{
		if(GameMainLoop.Instance != null)
			GameMainLoop.Instance.RegisterController(this);
	}
	
	public abstract IEnumerator Init ();
}

using UnityEngine;
using System.Collections;

public abstract class AbstractController : MonoBehaviour, IController
{
	protected virtual void Start()
	{
		if(GameMainLoop.Instance != null)
			GameMainLoop.Instance.RegisterController(this);
	}
	
	public abstract IEnumerator Init ();
}

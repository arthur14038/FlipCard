using UnityEngine;
using System.Collections;

public abstract class SingletonMonoBehavior<T> : MonoBehaviour
{	
	protected static T _instance;

	public virtual void Awake() {

		if(_instance == null || _instance.Equals(default(T)))
			_instance = (T)((System.Object)this);
	}
	
	public static T Instance {
		get { return _instance; }
	}
}


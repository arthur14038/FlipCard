using UnityEngine;
using System.Collections;

public abstract class AbstractView : MonoBehaviour, IView{
	public abstract IEnumerator Init ();
	protected static Vector2 hideUp = new Vector2(0f, 1664f);
	protected static Vector2 hideDown = new Vector2(0f, -1664f);
	protected static Vector2 hideRight = new Vector2(1080f, 0f);
	protected static Vector2 hideLeft = new Vector2(-1080f, 0f);
	protected VoidNoneParameter backEvent;

    public virtual void ShowUI (bool needAnimation)
	{
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);

		if(needAnimation && this.gameObject.activeInHierarchy)
			StartCoroutine(ShowUIAnimation());
	}

	public virtual void HideUI (bool needAnimation)
	{
		if(needAnimation)
		{
			if(this.gameObject.activeInHierarchy)
				StartCoroutine(HideUIAnimation());
		}else
		{
			if(this.gameObject.activeSelf)
				this.gameObject.SetActive(false);
		}
	}
		
	void Update()
	{
		if(backEvent != null && Input.GetKeyUp(KeyCode.Escape))
			backEvent();

        if(ScreenEffectManager.Instance != null && Input.GetMouseButtonDown(0))
        {
            Vector3 effectPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            effectPos.z = -5f;
            ParticleListener pl = ScreenEffectManager.Instance.GetScreenEffect();
            pl.ShowEffect(effectPos);
        }
	}

	protected abstract IEnumerator HideUIAnimation();

	protected abstract IEnumerator ShowUIAnimation();
}

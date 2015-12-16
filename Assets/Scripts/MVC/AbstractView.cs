using UnityEngine;
using System.Collections;

public abstract class AbstractView : MonoBehaviour, IView{
	public abstract IEnumerator Init ();
	protected static Vector2 hideUp = new Vector2(0f, 1664f);
	protected static Vector2 hideDown = new Vector2(0f, -1664f);
	protected static Vector2 hideRight = new Vector2(1080f, 0f);
	protected static Vector2 hideLeft = new Vector2(-1080f, 0f);
	protected VoidNoneParameter escapeEvent;
	protected Coroutine hideCoroutine;
	protected Coroutine showCoroutine;

	public virtual void ShowUI (bool needAnimation)
	{
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);

		if(needAnimation && this.gameObject.activeInHierarchy)
		{
			if(hideCoroutine != null)
			{
				StopCoroutine(hideCoroutine);
				hideCoroutine = null;
			}
			showCoroutine = StartCoroutine(ShowUIAnimation());
		}
	}

	public virtual void HideUI (bool needAnimation)
	{
		if(needAnimation)
		{
			if(this.gameObject.activeInHierarchy)
			{
				if(showCoroutine != null)
				{
					StopCoroutine(showCoroutine);
					showCoroutine = null;
				}
				hideCoroutine = StartCoroutine(HideUIAnimation());
			}				
		}else
		{
			if(this.gameObject.activeSelf)
				this.gameObject.SetActive(false);
		}
	}
		
	void Update()
	{
		if(escapeEvent != null && Input.GetKeyUp(KeyCode.Escape))
			escapeEvent();

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

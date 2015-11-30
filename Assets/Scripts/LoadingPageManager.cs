using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingPageManager : MonoBehaviour {
	Coroutine loadingAnimation;
	CanvasGroup thisCanvasGroup;

	public void Init()
	{
		thisCanvasGroup = this.GetComponent<CanvasGroup>();
	}

	void DoLoadingAnimation()
	{
		loadingAnimation = StartCoroutine(LoadingAnimation());
	}

	IEnumerator LoadingAnimation()
	{
		yield return new WaitForSeconds(0.2f);

		DoLoadingAnimation();
	}

	public IEnumerator TurnOn()
	{
		thisCanvasGroup.alpha = 0f;
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);
		
		DoLoadingAnimation();
		yield return thisCanvasGroup.DOFade(1, 0.3f).WaitForCompletion();
	}

	public IEnumerator FadeOutLoadingPage()
	{
		thisCanvasGroup.alpha = 1f;
		yield return thisCanvasGroup.DOFade(0, 0.3f).WaitForCompletion();
		
		if(loadingAnimation != null)
			StopCoroutine(loadingAnimation);
				
		if(this.gameObject.activeSelf)
			this.gameObject.SetActive(false);
	}
}

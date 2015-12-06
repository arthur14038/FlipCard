using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingPageManager : MonoBehaviour {
	Coroutine loadingAnimation;
	CanvasGroup thisCanvasGroup;
	public Image image_Green;
	public Image image_Orange;
	public Image image_Pink;
	public Image image_Blue;
	public Image image_Move;
	public Text text_Loading;
	Vector3 rotateAngle = new Vector3(0f, 0f, 120f);

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
		image_Move.rectTransform.anchoredPosition = image_Green.rectTransform.anchoredPosition;
		image_Move.color = image_Green.color;

		text_Loading.DOFade(0.6f, 0.5f).OnComplete(delegate(){
			text_Loading.DOFade(1f, 0.5f);
		}
		);

		image_Move.rectTransform.DOAnchorPos(image_Orange.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Orange.color, 0.25f);
		yield return image_Green.rectTransform.DOScaleX(0f, 3/24f).WaitForCompletion();
		yield return image_Green.rectTransform.DOScaleX(1.3f, 2/24f).WaitForCompletion();
		yield return image_Green.rectTransform.DOScaleX(1f, 1/24f).WaitForCompletion();

		image_Move.rectTransform.DOAnchorPos(image_Blue.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Blue.color, 0.25f).WaitForCompletion();
		yield return image_Orange.rectTransform.DOScaleX(0f, 3/24f).WaitForCompletion();
		yield return image_Orange.rectTransform.DOScaleX(1.3f, 2/24f).WaitForCompletion();
		yield return image_Orange.rectTransform.DOScaleX(1f, 1/24f).WaitForCompletion();

		image_Move.rectTransform.DOAnchorPos(image_Pink.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Pink.color, 0.25f).WaitForCompletion();
		yield return image_Blue.rectTransform.DOScaleX(0f, 3/24f).WaitForCompletion();
		yield return image_Blue.rectTransform.DOScaleX(1.3f, 2/24f).WaitForCompletion();
		yield return image_Blue.rectTransform.DOScaleX(1f, 1/24f).WaitForCompletion();

		image_Move.rectTransform.DOAnchorPos(image_Green.rectTransform.anchoredPosition, 0.25f);
		image_Move.DOColor(image_Green.color, 0.25f).WaitForCompletion();
		image_Pink.rectTransform.DOScale(1.3f, 1/24f).OnComplete(
			delegate(){
			image_Pink.rectTransform.DOScale(1f, 3/24f);
		}
		);
		image_Pink.rectTransform.DORotate(-rotateAngle, 3/24f).OnComplete(delegate(){
			image_Pink.rectTransform.DORotate(Vector3.zero, 7/24f);
		}
		);
		yield return new WaitForSeconds(0.25f);

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

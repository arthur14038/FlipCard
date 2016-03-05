using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ScoreText : MonoBehaviour {
	public Sprite[] scoreSprits;
	CanvasGroup thisCanvasGroup;
	Image thisImage;
    VoidScoreText recycle;

    public void Init(VoidScoreText recycle)
    {
        this.gameObject.SetActive(false);
        this.recycle = recycle;
		thisCanvasGroup = this.GetComponent<CanvasGroup>();
		thisImage = this.GetComponent<Image>();
	}
	
	public void ShowScoreText(Vector2 pos, int score)
	{
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);
		thisImage.rectTransform.anchoredPosition = pos;
		thisCanvasGroup.alpha = 1f;

		switch(score)
		{
			case 1:
				thisImage.sprite = scoreSprits[0];
                break;
			case 3:
				thisImage.sprite = scoreSprits[1];
				break;
			case 5:
				thisImage.sprite = scoreSprits[2];
				break;
			case 7:
				thisImage.sprite = scoreSprits[3];
				break;
		}

		StartCoroutine(ScoreTextEffect(pos.y));
	}

	IEnumerator ScoreTextEffect(float y)
	{
		thisImage.rectTransform.DOAnchorPosY(y + 50f, 0.8f).SetEase(Ease.OutQuad);
		yield return thisCanvasGroup.DOFade(0f, 0.2f).SetDelay(0.6f).WaitForCompletion();
		this.gameObject.SetActive(false);
		if(recycle != null)
			recycle(this);
	}
}

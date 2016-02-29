using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ScoreText : MonoBehaviour {
	public RectTransform text_Combo;
	public RectTransform text_GoldCard;
	Vector2 additionScorePosition1 = new Vector2(-46f, -35f);
	Vector2 additionScorePosition2 = new Vector2(-46f, -105f);
	CanvasGroup thisCanvasGroup;
	RectTransform thisRectTransform;
    VoidScoreText recycle;

    public void Init(VoidScoreText recycle)
    {
        this.gameObject.SetActive(false);
        this.recycle = recycle;
		thisCanvasGroup = this.GetComponent<CanvasGroup>();
		thisRectTransform = this.GetComponent<RectTransform>();
    }
	
	public void ShowScoreText(Vector2 pos, bool comboAward, bool goldCardAward)
	{
		if(!this.gameObject.activeSelf)
			this.gameObject.SetActive(true);
		thisRectTransform.anchoredPosition = pos;
		thisCanvasGroup.alpha = 1f;

		if(comboAward)
		{
			text_Combo.gameObject.SetActive(true);
			text_Combo.anchoredPosition = additionScorePosition1;
            if(goldCardAward)
			{
				text_GoldCard.gameObject.SetActive(true);
				text_GoldCard.anchoredPosition = additionScorePosition2;
			} else
			{
				text_GoldCard.gameObject.SetActive(false);
			}
		}else
		{
			text_Combo.gameObject.SetActive(false);
			if(goldCardAward)
			{
				text_GoldCard.gameObject.SetActive(true);
				text_GoldCard.anchoredPosition = additionScorePosition1;
			} else
			{
				text_GoldCard.gameObject.SetActive(false);
			}
		}

		StartCoroutine(ScoreTextEffect(pos.y));
	}

	IEnumerator ScoreTextEffect(float y)
	{
		thisRectTransform.DOAnchorPosY(y + 50f, 0.8f).SetEase(Ease.OutQuad);
		yield return thisCanvasGroup.DOFade(0f, 0.2f).SetDelay(0.6f).WaitForCompletion();
		this.gameObject.SetActive(false);
		if(recycle != null)
			recycle(this);
	}
}

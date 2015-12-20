using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ShowComplimentTool : MonoBehaviour {
	public Sprite[] complimentSprits;
	public Image[] image_Compliments;
	public Image image_BG;
	Vector2 hideLeft;
	Vector2 hideRight;
	RectTransform thisRectTransform;

	public void Init(Vector2 hideLeft, Vector3 hideRight)
	{
		this.hideLeft = hideLeft;
		this.hideRight = hideRight;
		thisRectTransform = this.GetComponent<RectTransform>();
		this.gameObject.SetActive(false);
	}

	public void ShowCompliment(int value)
	{
		this.gameObject.SetActive(true);
		int goldCount = value / 9;
		int silverCount = (value % 9) / 3;
		int copperCount = value % 3;
		Vector2 size = image_BG.rectTransform.sizeDelta;
		size.x = 100 + 135 * (goldCount + silverCount + copperCount);
		image_BG.rectTransform.sizeDelta = size;
		for(int i = 0 ; i < image_Compliments.Length ; ++i)
		{
			image_Compliments[i].gameObject.SetActive(true);
			if(goldCount > 0)
			{
				image_Compliments[i].sprite = complimentSprits[2];
				--goldCount;
            }else if(silverCount > 0)
			{
				image_Compliments[i].sprite = complimentSprits[1];
				--silverCount;
			}else if(copperCount > 0)
			{
				image_Compliments[i].sprite = complimentSprits[0];
				--copperCount;
			}else
			{
				image_Compliments[i].gameObject.SetActive(false);
            }
		}

		StartCoroutine(ShowEffect());
	}

	IEnumerator ShowEffect()
	{
		thisRectTransform.anchoredPosition = hideRight;
		yield return thisRectTransform.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
		yield return thisRectTransform.DOAnchorPos(hideLeft, 0.5f).SetDelay(0.3f).SetEase(Ease.InBack).WaitForCompletion();
		this.gameObject.SetActive(false);
	}
}

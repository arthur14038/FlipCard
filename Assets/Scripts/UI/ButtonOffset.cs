using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonOffset : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	public Vector2 offset = Vector2.zero;
	public Vector3 scale = Vector3.one;
	public float duration = 0.1f;
	public Ease offsetEase = Ease.Linear;
	public Ease scaleEase = Ease.Linear;
	protected RectTransform thisRectTransform;
	Vector2 oriPos;
	bool shouldWork = false;
	bool doScale = false;
	bool doOffset = false;

	protected virtual void Start()
	{
		thisRectTransform = this.GetComponent<RectTransform>();
		if(thisRectTransform == null)
		{
			shouldWork = false;
		}else
		{
			if(duration > 0f)
			{
				if(offset == Vector2.zero && scale == Vector3.one)
				{
					shouldWork = false;
				}else
				{
					shouldWork = true;

					if(offset != Vector2.zero)
						doOffset = true;
					else
						doOffset = false;

					if(scale != Vector3.one)
						doScale = true;
					else
						doScale = false;
				}
			}else
			{
				shouldWork = false;
			}
		}
	}

	public virtual void OnPointerDown (PointerEventData eventData)
	{
		if(shouldWork)
		{
			oriPos = thisRectTransform.anchoredPosition;

			if(doOffset)
				thisRectTransform.DOAnchorPos(oriPos+offset, duration).SetEase(offsetEase);

			if(doScale)
				thisRectTransform.DOScale(scale, duration).SetEase(scaleEase);
		}
	}

	public virtual void OnPointerUp (PointerEventData eventData)
	{
		if(shouldWork)
		{
			if(doOffset)
				thisRectTransform.DOAnchorPos(oriPos, duration).SetEase(offsetEase);
			
			if(doScale)
				thisRectTransform.DOScale(Vector3.one, duration).SetEase(scaleEase);
		}
	}
}

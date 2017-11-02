using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler 
{

	private Image bgImg;
	private Image joyStickImg;
	public Vector3 inputVector;

	void Start()
	{
		bgImg = GetComponent<Image> ();
		joyStickImg = transform.GetChild (0).GetComponent<Image> ();
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		Vector2 pos;
		if  (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos))
		{
			pos.x = pos.x / bgImg.rectTransform.sizeDelta.x;
			pos.y = pos.y / bgImg.rectTransform.sizeDelta.y;

			inputVector = new Vector3 (pos.x*2+1, 0, pos.y*2-1);
			inputVector = (inputVector.sqrMagnitude > 1f) ? inputVector.normalized : inputVector;

			joyStickImg.rectTransform.anchoredPosition = new Vector3 (inputVector.x * (bgImg.rectTransform.sizeDelta.x/3), inputVector.z * (bgImg.rectTransform.sizeDelta.y/3));

		}
	}

	public virtual void OnPointerDown(PointerEventData ped)
	{
		OnDrag (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		inputVector = Vector3.zero;
		joyStickImg.rectTransform.anchoredPosition = Vector3.zero;
	}


}

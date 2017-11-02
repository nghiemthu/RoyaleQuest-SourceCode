using UnityEngine;
using System.Collections;

public class Crosshairs : MonoBehaviour {

	public LayerMask targetMask;
	public SpriteRenderer dot;
	public Color dotHighlightColour;
	Color originalDotColour;
	SpriteRenderer crosshair;

	void Start() {
		Cursor.visible = false;
		originalDotColour = dot.color;
		crosshair = GetComponent<SpriteRenderer> ();
	}
	
	void Update () {
		transform.Rotate (Vector3.forward * -40 * Time.deltaTime);
	}

	public void DetectTargets(Ray ray) {
		if (Physics.Raycast (ray, 100, targetMask)) {
			dot.color = dotHighlightColour;
			crosshair.color = dotHighlightColour;
		} else {
			dot.color = originalDotColour;
			crosshair.color = Color.white;
		}
	}
}

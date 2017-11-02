using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	float lifetime = 2;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

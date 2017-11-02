using UnityEngine;
using System.Collections;

public class Power : MonoBehaviour {

	float timeToDestroy;
	public float timeBetweenToDestroy;

	protected virtual void Awake(){
		timeToDestroy = Time.time + timeBetweenToDestroy;
	}

	protected virtual void Update(){
		
		if (Time.time > timeToDestroy) {
			StartCoroutine (DestroyPower());

		}
	}

	IEnumerator DestroyPower(){
		float spawnDelay = 1f;
		float tileFlashSpeed = 4;

		Material tileMat = GetComponent<Renderer> ().material;
		Color initialColour = tileMat.color;
		Color flashColour = Color.clear;
		float spawnTimer = 0;

		while (spawnTimer < spawnDelay) {

			tileMat.color = Color.Lerp(initialColour,flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1f));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Destroy (gameObject);
	}
}

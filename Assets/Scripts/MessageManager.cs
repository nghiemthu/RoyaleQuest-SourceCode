using UnityEngine;
using System.Collections;

public class MessageManager : MonoBehaviour {

	public string[] questMessage;
	public int characterindex;

	void OnCollisionEnter(Collision target){
		if (target.gameObject.tag == "Player"){
			HomeUI.instance.OnMessageCollision (questMessage[(int)Random.Range(0, questMessage.Length -0.1f)], characterindex);
		}
	}

	void OnCollisionExit(Collision target){
		if (target.gameObject.tag == "Player"){
			HomeUI.instance.OnMessageExit (characterindex);
		}
	}
}

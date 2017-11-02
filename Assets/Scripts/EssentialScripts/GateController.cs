using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour {


	void OnTriggerEnter(Collider target){
		if (target.tag == "Player"){
			if (PlayerPrefsController.instance.GetBook() > 0 && PlayerPrefsController.instance.GetDone() ==0) {
				HomeUI.instance.StartNewGame ();
			} else if (PlayerPrefsController.instance.GetBook() == 0 ) {
				HomeUI.instance.AnimateTalkPanel ("You do not have enough book to revive!", 0);
			} else if (PlayerPrefsController.instance.GetDone() ==1){
				HomeUI.instance.AnimateTalkPanel ("You have destroyed them all! Wait for the next update", 0);
			}
		}
	}
	void OnTriggerExit(Collider target){
		if (target.tag == "Player") {
			HomeUI.instance.OnMessageExit ( 0);
		}
	}
}

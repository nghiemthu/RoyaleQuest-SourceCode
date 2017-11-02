using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TimeController : MonoBehaviour {

	public Text timer;
	private static DateTime energyRefillTime;

	void Start(){
		if (!PlayerPrefs.HasKey ("FirstTime")) {
			SetEnergyRefillTimer ();
			PlayerPrefs.SetInt ("FirstTime", 0);
		}

		DateTime.TryParse(PlayerPrefsController.instance.GetenergyRefillTime (),out energyRefillTime );
		if (energyRefillTime < DateTime.Now){
			if (DateTime.Now> energyRefillTime.AddMinutes(5) && PlayerPrefsController.instance.GetBook() <5 ){
				PlayerPrefsController.instance.SetBook (PlayerPrefsController.instance.GetBook () + 1);
			}
			if (DateTime.Now> energyRefillTime.AddMinutes(10) && PlayerPrefsController.instance.GetBook() <5 ){
				PlayerPrefsController.instance.SetBook (PlayerPrefsController.instance.GetBook () + 1);
			}
			if (DateTime.Now> energyRefillTime.AddMinutes(15)&& PlayerPrefsController.instance.GetBook() <5 ){
				PlayerPrefsController.instance.SetBook (PlayerPrefsController.instance.GetBook () + 1);
			}
			if (DateTime.Now> energyRefillTime.AddMinutes(20) && PlayerPrefsController.instance.GetBook() <5 ){
				PlayerPrefsController.instance.SetBook (PlayerPrefsController.instance.GetBook () + 1);
			}
			if (DateTime.Now> energyRefillTime.AddMinutes(25)&& PlayerPrefsController.instance.GetBook() <5 ){
				PlayerPrefsController.instance.SetBook (PlayerPrefsController.instance.GetBook () + 1);
			}


		}
	}
	void Update(){
		if (PlayerPrefsController.instance.GetBook() == 5){
			timer.text = "Full";
		}
		if (PlayerPrefsController.instance.GetBook() < 5 && PlayerPrefsController.instance.GetBook() >=0){
			CheckIfEnergyWaitTimeOver ();
		}

	}

	private static void SetEnergyRefillTimer() {
		energyRefillTime = DateTime.Now.AddMinutes(5);
		PlayerPrefsController.instance.SetenergyRefillTime (energyRefillTime.ToString());
	}
		

	public void CheckIfEnergyWaitTimeOver() {
		if (DateTime.Now < energyRefillTime ) {
			
				TimeSpan remaining = energyRefillTime - DateTime.Now;
				var timerCountdownText = string.Format("{0:D2}:{1:D2}", remaining.Minutes, remaining.Seconds);
				timer.text = timerCountdownText;

			} else {

			if (PlayerPrefsController.instance.GetBook() <5){
				PlayerPrefsController.instance.SetBook (PlayerPrefsController.instance.GetBook () + 1);
				SetEnergyRefillTimer ();
			}
		}
			
	}
}
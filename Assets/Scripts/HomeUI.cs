using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class HomeUI : MonoBehaviour {

	public RectTransform talkPanel;
	public Text messageText;
	public Image messageImage;
	public Image playerImage;
	public Image playerTrainingImage;
	public Sprite[] characterSprites;
	public Sprite[] playerSprites;
	public Text timer;
	public RectTransform[] infoPanel;
	public RectTransform training;
	public Text levelText,levelText2 ;
	public Text xpText, xpText2;
	public Text questText;
	public Text bookText;
	GameController controller;

	//TrainingPanel
	public Text playerHp;
	public Text playerPower;
	public Text playerRunSpeed;
	public Text projectileSpeed;
	public Text projectileDamage;
	public Text upgrade;

	public static HomeUI instance;

	void Awake(){
		SetTextElements ();
	}
	void Start () {
		Cursor.visible = true;
		controller = FindObjectOfType<GameController> ();
		if (instance == null){
			instance = this;
		}


		if (PlayerPrefsController.instance.GetQuestIndex() == 0){
			AnimateTalkPanel ("Many monsters appear in the forest now! You should hurry up!", 0);
		}  
		if (PlayerPrefsController.instance.GetQuestIndex() == 10){
			AnimateTalkPanel ("Great job! But more powerful mosters are located at the graveyard!", 0);
		} 
		if (PlayerPrefsController.instance.GetQuestIndex() == 20){
			AnimateTalkPanel ("You are really close to ultimate victory, Come and defeat their king now!", 0);
		} 
		if (PlayerPrefsController.instance.GetQuestIndex() == 29){
			AnimateTalkPanel ("You just destroy them all! Wait for next update!", 0);
		}

	}
	
	void Update () {
		upgrade.text = "Upgade: " + PlayerPrefsController.instance.GetUpgrade ();
		bookText.text = "" + PlayerPrefsController.instance.GetBook ();
		playerHp.text ="Health: " + PlayerPrefsController.instance.GetPlayerHealth () +"";
		playerPower.text = "Power: "+PlayerPrefsController.instance.GetPlayerPower ()+"";
		playerRunSpeed.text = "Run Speed: "+ PlayerPrefsController.instance.GetPlayerRunSpeed ()+"";
		projectileSpeed.text ="Gun Speed: "+ PlayerPrefsController.instance.GetProjectileSpeed ()+"";
		projectileDamage.text ="Damage: "+ PlayerPrefsController.instance.GetProjectileDamage ()+"";
	}

	void SetTextElements(){
		levelText.text = "Level " + ((int)(PlayerPrefsController.instance.GetXP ()/1000) +1);
		xpText.text = "XP: "+ (PlayerPrefsController.instance.GetXP () - ((int)(PlayerPrefsController.instance.GetXP ()/1000)*1000)) + "/1000";
		levelText2.text = "Level " + ((int)(PlayerPrefsController.instance.GetXP ()/1000) +1);
		xpText2.text = "XP: "+ (PlayerPrefsController.instance.GetXP () - ((int)(PlayerPrefsController.instance.GetXP ()/1000)*1000)) + "/1000";
		questText.text = "Quest " + (PlayerPrefsController.instance.GetQuestIndex () +1);
		playerImage.sprite = playerSprites[PlayerPrefsController.instance.GetPlayer()];
		playerTrainingImage.sprite = playerSprites [PlayerPrefsController.instance.GetPlayer ()];
	}

	public void AnimateTalkPanel(string message, int playerIndex){
		messageText.text = "" + message ;
		messageImage.sprite = characterSprites [playerIndex];
		StartCoroutine (AnimateTalkPanel());
	}

	IEnumerator AnimateTalkPanel() {

		float delayTime =4.5f;
		float speed = 3f;
		float animatePercent = 0;
		int dir = 1;

		float endDelayTime = Time.time + 1 / speed + delayTime;

		while (animatePercent >= 0) {
			animatePercent += Time.deltaTime * speed * dir;

			if (animatePercent >= 1) {
				animatePercent = 1;
				if (Time.time > endDelayTime) {
					dir = -1;
				}
			}

			talkPanel.anchoredPosition = Vector2.up * Mathf.Lerp (-350, 200, animatePercent);
			yield return null;
		}

	}

	public void OnMessageCollision(string message, int characterIndex){
		messageText.text =  message;
		messageImage.sprite = characterSprites[characterIndex];
			StartCoroutine (AnimateTalkPanel(1));
			if (characterIndex == 1){
				if (PlayerPrefsController.instance.GetQuestIndex () <= 10) {
					StartCoroutine (AnimateInfoPanel(infoPanel[0],1));
				} else if (PlayerPrefsController.instance.GetQuestIndex () <= 20) {
					StartCoroutine (AnimateInfoPanel(infoPanel[1],1));
				} else {
					StartCoroutine (AnimateInfoPanel(infoPanel[2],1));
				}
			}

			if (characterIndex == 2){
				StartCoroutine (AnimateInfoPanel(training,1));
			}
	}

	public void OnMessageExit(int characterIndex){
		Debug.Log ("Im here");
		StartCoroutine (AnimateTalkPanel(-1));
		if (characterIndex == 1){
			if (PlayerPrefsController.instance.GetQuestIndex () <= 10) {
				StartCoroutine (AnimateInfoPanel(infoPanel[0], -1));
			} else if (PlayerPrefsController.instance.GetQuestIndex () <= 20) {
				StartCoroutine (AnimateInfoPanel(infoPanel[1], -1));
			} else {
				StartCoroutine (AnimateInfoPanel(infoPanel[2], -1));
			}
		}

		if (characterIndex == 2){
			StartCoroutine (AnimateInfoPanel(training, -1));
		}
	}

	IEnumerator AnimateTalkPanel(int dir) {

		float speed = 2f;
		float animatePercent = 0;

		if (dir == 1){
			animatePercent = 0;
		} else if (dir == -1){
			animatePercent = 1;
		}

		while (animatePercent >= 0 && animatePercent <=1) {
			animatePercent += Time.deltaTime * speed * dir ;
			talkPanel.anchoredPosition = Vector2.up * Mathf.Lerp (-350, 200, animatePercent);
			yield return null;
		}
	}
		

	IEnumerator AnimateInfoPanel(RectTransform panelToAnimate, int dir) {

		float speed = 2f;
		float animatePercent = 0;

		if (dir == 1){
			animatePercent = 0;
		} else if (dir == -1){
			animatePercent = 1;
		}

		while (animatePercent >= 0 && animatePercent <=1) {
			animatePercent += Time.deltaTime * speed * dir;

			panelToAnimate.anchoredPosition = Vector2.left * Mathf.Lerp (-1300, 0, animatePercent);
			yield return null;
		}
	}
	//Training
	public void IncreaseMaxHealth(){
		if (PlayerPrefsController.instance.GetUpgrade () > 0) {
			PlayerPrefsController.instance.SetPlayerHealth (PlayerPrefsController.instance.GetPlayerHealth () + 1);
			PlayerPrefsController.instance.SetUpgrade (PlayerPrefsController.instance.GetUpgrade () - 1);
		} else {
			messageText.text = "You need to level up first";
		}

	}
	public void IncreaseMaxPower(){
		if (PlayerPrefsController.instance.GetUpgrade() > 0){
			PlayerPrefsController.instance.SetPlayerPower (PlayerPrefsController.instance.GetPlayerPower() + 1);
			PlayerPrefsController.instance.SetUpgrade (PlayerPrefsController.instance.GetUpgrade()-1);
		} else {
			messageText.text = "You need to level up first";
		}
	}
	public void IncreaseRunSpeed(){
		if (PlayerPrefsController.instance.GetUpgrade() > 0){
			PlayerPrefsController.instance.SetPlayerRunSpeed (PlayerPrefsController.instance.GetPlayerRunSpeed() + 0.25f);
			PlayerPrefsController.instance.SetUpgrade (PlayerPrefsController.instance.GetUpgrade()-1);
		} else {
			messageText.text = "You need to level up first";
		}
	}
	public void IncreaseProjectileSpeed(){
		if (PlayerPrefsController.instance.GetUpgrade() > 0){
			PlayerPrefsController.instance.SetProjectileSpeed (PlayerPrefsController.instance.GetProjectileSpeed() + 1);
			PlayerPrefsController.instance.SetUpgrade (PlayerPrefsController.instance.GetUpgrade()-1);
		} else {
			messageText.text = "You need to level up first";
		}
	}
	public void IncreaseDamage(){
		if (PlayerPrefsController.instance.GetUpgrade() > 0){
			PlayerPrefsController.instance.SetProjectileDamage (PlayerPrefsController.instance.GetProjectileDamage() + 0.25f);
			PlayerPrefsController.instance.SetUpgrade (PlayerPrefsController.instance.GetUpgrade()-1);
		} else {
			messageText.text = "You need to level up first";
		}
	}

	// UI Input
	public void StartNewGame() {
		SceneManager.LoadScene ("GamePlay");
	}

	public void ReturnToMainMenu() {
		SceneManager.LoadScene ("Home");
	}

}

using UnityEngine;
using System.Collections;
using System;

public class PlayerPrefsController : MonoBehaviour {

	public static PlayerPrefsController instance;

	private const string quest = "Quest";
	private const string xp = "Xp";
	private const string book = "Book";
	private const string player = "Player";
	private const string energyRefillTime = "energyRefillTime";
	private const string currentQuestNumber ="CurrentQuestNumber";
	private const string playerHp = "playerHp";
	private const string playerPower = "playerPower";
	private const string playerRunSpeed = "playerRunSpeed";
	private const string projectileSpeed = "projectileSpeed";
	private const string projectileDamge = "projectileDamage";
	private const string upgrade = "upgrade";
	private const string done = "done";

	void Awake (){
		Screen.SetResolution (480, 720, false);
		 //PlayerPrefs.DeleteAll ();
		//PlayerPrefs.SetInt (book,5);
		//PlayerPrefs.SetInt (upgrade,15);
		//PlayerPrefs.SetInt (quest,28);
		//PlayerPrefs.SetInt (player,2);
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		isGameStartedFirstTime();
	}
	
	void isGameStartedFirstTime(){
		if(!PlayerPrefs.HasKey("GamePlayFirstTime")){
			PlayerPrefs.SetInt (quest,0);
			PlayerPrefs.SetInt (upgrade,0);
			PlayerPrefs.SetInt (xp,0);
			PlayerPrefs.SetInt (done,0);
			PlayerPrefs.SetInt (book,5);
			PlayerPrefs.SetInt (player,2);
			PlayerPrefs.SetInt (playerHp, 10);
			PlayerPrefs.SetInt (playerPower, 10);
			PlayerPrefs.SetFloat (playerRunSpeed, 4);
			PlayerPrefs.SetFloat (projectileDamge, 1);
			PlayerPrefs.SetInt (projectileSpeed, 15);
			PlayerPrefs.SetInt (currentQuestNumber,0);
			PlayerPrefs.SetInt ("GamePlayFirstTime", 1);
		}
	}

	public void SetQuestIndex(int questIndex){
		PlayerPrefs.SetInt (quest, questIndex);
	}

	public int GetQuestIndex(){
		return PlayerPrefs.GetInt (quest);
	}

	public void SetCurrentQuestNumber(int CurrentQuestNumber){
		PlayerPrefs.SetInt (currentQuestNumber, CurrentQuestNumber);
	}

	public int GetCurrentQuestNumber(){
		return PlayerPrefs.GetInt (currentQuestNumber);
	}

	public void SetBook(int _book){
		PlayerPrefs.SetInt (book, _book);
	}

	public int GetBook(){
		return PlayerPrefs.GetInt (book);
	}

	public void SetXP(int _XP){
		PlayerPrefs.SetInt (xp, _XP);
	}

	public int GetXP(){
		return PlayerPrefs.GetInt (xp);
	}

	public void SetPlayer(int _player){
		PlayerPrefs.SetInt (player, _player);
	}

	public int GetPlayer(){
		return PlayerPrefs.GetInt (player);
	}

	public void SetenergyRefillTime(String _energyRefillTime){
		PlayerPrefs.SetString (energyRefillTime, _energyRefillTime);
	}

	public string GetenergyRefillTime(){
		return PlayerPrefs.GetString (energyRefillTime);
	}

	public void SetPlayerHealth(int _playerhealth){
		PlayerPrefs.SetInt (playerHp, _playerhealth);
	}

	public int GetPlayerHealth(){
		return PlayerPrefs.GetInt (playerHp);
	}

	public void SetPlayerPower(int _playerPower){
		PlayerPrefs.SetInt (playerPower, _playerPower);
	}

	public int GetPlayerPower(){
		return PlayerPrefs.GetInt (playerPower);
	}

	public void SetPlayerRunSpeed(float _playerRunSpeed){
		PlayerPrefs.SetFloat (playerRunSpeed, _playerRunSpeed);
	}

	public float GetPlayerRunSpeed(){
		return PlayerPrefs.GetFloat (playerRunSpeed);
	}

	public void SetProjectileSpeed(int _projectileSpeed){
		PlayerPrefs.SetInt (projectileSpeed, _projectileSpeed);
	}

	public int GetProjectileSpeed(){
		return PlayerPrefs.GetInt (projectileSpeed);
	}

	public void SetProjectileDamage(float _projectileDamage){
		PlayerPrefs.SetFloat (projectileDamge, _projectileDamage);
	}

	public float GetProjectileDamage(){
		return PlayerPrefs.GetFloat(projectileDamge);
	}

	public void SetUpgrade(int _upgrade){
		PlayerPrefs.SetInt (upgrade, _upgrade);
	}

	public int GetUpgrade(){
		return PlayerPrefs.GetInt (upgrade);
	}
	public void SetDone(int _done){
		PlayerPrefs.SetInt (done, _done);
	}

	public int GetDone(){
		return PlayerPrefs.GetInt (done);
	}

}

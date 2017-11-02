using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIControl : MonoBehaviour {

	public static GameUIControl instance;
	public RectTransform healthBar;
	public RectTransform powerBar;
	public RectTransform innerPowerBar;
	public Sprite[] playerSprites;
	public Image playerImage;
	public RectTransform talkPanel;
	public Text messageText;
	public Image messageImage;
	public Sprite[] characterSprites;
	public Text xpText;
	public Image shardImage;
	public Sprite[] shardSprites;
	public Text questText;
	public Text waveText;
	public Text enemyCountText;
	Color initialHealthBarColor;
	Color initialPowerBarColor;

	Player player;

	// Use this for initialization
	void Start () {
		if (instance == null){
			instance = this;
		}
		initialHealthBarColor = healthBar.GetComponent<Image> ().color;
		initialPowerBarColor = powerBar.GetComponent<Image> ().color;
		xpText.gameObject.SetActive (false);
		shardImage.gameObject.SetActive (false);
		player = FindObjectOfType<Player> ();
		player.OnDeath += OpenGameCompletePanel;
		playerImage.sprite = playerSprites[PlayerPrefsController.instance.GetPlayer()];
		questText.text = "Quest " + (PlayerPrefsController.instance.GetQuestIndex ()+1);

	}

	// Update is called once per frame
	void Update () {
		float healthPercent = 0;
		if (player != null) {
			healthPercent = player.health / player.startingHealth;
		}
		healthBar.localScale = new Vector3 (healthPercent, 1, 1);

		float powerPercent = 0;
		if (player != null) {
			powerPercent = player.power / PlayerPrefsController.instance.GetPlayerPower ();
		}
		powerBar.localScale = new Vector3 (powerPercent, 1, 1);

		float innerPowerPercent = 0;
		if (player != null) {
			innerPowerPercent = player.innerPower / 15;
		}
		innerPowerBar.localScale = new Vector3 (innerPowerPercent, 1, 1);
	}

	public void SetEnemyCountText(int enemyCount){
		enemyCountText.text = "Enemy " + enemyCount;
	}

	public void SetWaveText(int wave){
		waveText.text = "Wave  " + wave;
	}

	public void SetBarColor(){
		StartCoroutine (SetBarColorAnim());
	}

	IEnumerator SetBarColorAnim(){
		
		healthBar.GetComponent<Image> ().color = Color.yellow;
		powerBar.GetComponent<Image> ().color = Color.yellow;
		yield return new WaitForSeconds (4f);
		healthBar.GetComponent<Image> ().color = initialHealthBarColor;
		powerBar.GetComponent<Image> ().color = initialPowerBarColor;
	} 
	public void AnimateTalkPanel(string message,int playerIndex){
		messageText.text = "" + message ;
		messageImage.sprite = characterSprites [playerIndex];
		StartCoroutine (AnimateTalkPanel());
	}

	IEnumerator AnimateTalkPanel() {

		float delayTime =2.5f;
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

			talkPanel.anchoredPosition = Vector2.up * Mathf.Lerp (-170, 120, animatePercent);
			yield return null;
		}

	}

	public void AnimateXpText(){
		Color initialTextColor = xpText.color;
		StartCoroutine (AnimateXPText(Color.clear, initialTextColor, 1f));
	}

	public void AnimateShards(int shardIndex){
		Color initialTextColor = shardImage.color;
		shardImage.sprite = shardSprites [shardIndex];
		StartCoroutine (AnimateShards(Color.clear, initialTextColor, 1f));
	}

	IEnumerator AnimateXPText(Color from, Color to, float time) {
		xpText.gameObject.SetActive (true);
		float speed = 1/time;
		float animatePercent = 0;
	
		while (animatePercent <= 1 ) {
			animatePercent += Time.deltaTime * speed;
			xpText.color = Color.Lerp(from,to, animatePercent);
			xpText.rectTransform.anchoredPosition = Vector2.up * Mathf.Lerp (0, 150, animatePercent);
			yield return null;
		}
		yield return new WaitForSeconds (.5f);

		while (animatePercent >0){
			animatePercent -= Time.deltaTime * speed*2;
			xpText.color = Color.Lerp(from,to, animatePercent);
			yield return null;
		}
		xpText.color = to;
		xpText.gameObject.SetActive (false);
	}

	IEnumerator AnimateShards(Color from, Color to, float time) {
		shardImage.gameObject.SetActive (true);
		float speed = 1/time;
		float animatePercent = 0;

		while (animatePercent <= 1 ) {
			animatePercent += Time.deltaTime * speed*2;
			shardImage.color = Color.Lerp(from,to, animatePercent);
			shardImage.rectTransform.anchoredPosition = Vector2.up * Mathf.Lerp (0, 150, animatePercent);
			yield return null;
		}
		yield return new WaitForSeconds (.5f);

		while (animatePercent >0){
			animatePercent -= Time.deltaTime * speed*2;
			shardImage.color = Color.Lerp(from,to, animatePercent);
			yield return null;
		}
		shardImage.color = to;
		shardImage.gameObject.SetActive (false);
	}

	public void OpenGameCompletePanel(){
		Cursor.visible = true;
	}

	public void GoHome(){
		SceneManager.LoadScene ("Home");
	}
}

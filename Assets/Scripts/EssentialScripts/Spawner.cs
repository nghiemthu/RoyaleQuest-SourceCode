using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public bool devMode;

	public Wave[] waves;


	LivingEntity playerEntity;
	Transform playerT;

	Wave currentWave;
	int currentWaveNumber;

	int enemiesRemainingToSpawn;
	int enemiesRemainingAlive;
	float nextSpawnTime;

	MapGenerator map;

	float timeBetweenCampingChecks = 2;
	float campThresholdDistance = 1.5f;
	float nextCampCheckTime, nextCampCheckTimeTooLong;
	Vector3 campPositionOld;
	bool isCamping, isCampingTooLong;
	int randomIndex;
	bool isDisabled;
	bool spawnedBoss;
	bool kingDeath;
	bool isTooLong;
	public Enemy reaper;

	Player player;
	Transform kingHolder;
	 Enemy kingBoss;

	void Start() {
		isCampingTooLong = false;
		playerEntity = FindObjectOfType<Player> ();
		playerT = playerEntity.transform;
		player = FindObjectOfType<Player> ();

		nextCampCheckTime = timeBetweenCampingChecks + Time.time;
		nextCampCheckTimeTooLong = Time.time + timeBetweenCampingChecks*4;
		campPositionOld = playerT.position;
		playerEntity.OnDeath += OnPlayerDeath;

		map = FindObjectOfType<MapGenerator> ();
		NextWave ();

	}

	void Update() {
		if (!isDisabled) {
			if (Time.time > nextCampCheckTime) {
				nextCampCheckTime = Time.time + timeBetweenCampingChecks;

				isCamping = (Vector3.Distance (playerT.position, campPositionOld) < campThresholdDistance);
				campPositionOld = playerT.position;

			}

			if (Time.time > nextCampCheckTimeTooLong) {
				nextCampCheckTimeTooLong = Time.time + timeBetweenCampingChecks*5;

				isCampingTooLong = (Vector3.Distance (playerT.position, campPositionOld) < .2f);
				campPositionOld = playerT.position;

			}
			if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime) {
				enemiesRemainingToSpawn--;
				nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
				randomIndex = (int)Random.Range (0, currentWave.enemy.Length-.1f);
				if (spawnedBoss == true){
					randomIndex = (int)Random.Range (0, currentWave.enemy.Length-1.1f);
				}
				StartCoroutine ("SpawnEnemy");
			}

		}
			
		if (devMode) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				StopCoroutine("SpawnEnemy");
				foreach (Enemy enemy in FindObjectsOfType<Enemy>()) {
					GameObject.Destroy(enemy.gameObject);
				}
				NextWave();
			}
		}
	}

	IEnumerator SpawnEnemy() {
		float spawnDelay = .5f;
		float tileFlashSpeed = 4;

		Transform spawnTile = map.GetRandomOpenTile ();
		if (isCamping) {
			spawnTile = map.GetTileFromPosition(playerT.position);
		}
		Material tileMat = spawnTile.GetComponent<Renderer> ().material;
		Color initialColour = tileMat.color;
		Color flashColour = Color.black;
		float spawnTimer = 0;

		while (spawnTimer < spawnDelay) {

			tileMat.color = Color.Lerp(initialColour,flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, .5f));

			spawnTimer += Time.deltaTime;
			yield return null;
		}
		if (isCampingTooLong && !isTooLong) {
			Enemy spawnedEnemy = Instantiate (reaper, new Vector3 (spawnTile.position.x, 0, spawnTile.position.z), Quaternion.identity) as Enemy;
			spawnedEnemy.OnDeath += OnEnemyDeath;
			isTooLong = true;
			spawnedEnemy.SetCharacteristics (currentWave.skinColour [randomIndex], randomIndex);
		} else {
			Enemy spawnedEnemy = Instantiate (currentWave.enemy [randomIndex], new Vector3 (spawnTile.position.x, 0, spawnTile.position.z), Quaternion.identity) as Enemy;
			spawnedEnemy.OnDeath += OnEnemyDeath;

			if (spawnedEnemy.tag == "Boss" || spawnedEnemy.tag == "KingBoss"){
				spawnedBoss = true;
				if (spawnedEnemy.tag == "KingBoss"){
					kingBoss = spawnedEnemy;
					spawnedEnemy.OnDeath += OnKingDeath;
					kingHolder = kingBoss.transform.GetChild (0);
					StartCoroutine (KingSpawnEnemy());
					kingDeath = false;
				}
			}
			spawnedEnemy.SetCharacteristics (currentWave.skinColour [randomIndex], randomIndex);
		}

	}

	IEnumerator KingSpawnEnemy() {
		
		if (!kingDeath) {
			
			Enemy spawnedEnemy = Instantiate (currentWave.bossCEnemy, kingHolder.position, Quaternion.identity) as Enemy;
			spawnedEnemy.OnDeath += OnEnemyDeath;
			enemiesRemainingAlive++;
			GameUIControl.instance.SetEnemyCountText (enemiesRemainingAlive);
			spawnedEnemy.SetCharacteristics (Color.white, 6);
			yield return new WaitForSeconds (2f);
			StartCoroutine (KingSpawnEnemy());

		}
		yield return null;

	}
		

	void OnPlayerDeath() {
		isDisabled = true;
	}

	void OnKingDeath(){
		GameUIControl.instance.SetEnemyCountText (enemiesRemainingAlive);
		if (enemiesRemainingAlive == 0) {
			NextWave();
		}
		kingDeath = true;
	}

	void OnEnemyDeath() {
		enemiesRemainingAlive --;
		GameUIControl.instance.SetEnemyCountText (enemiesRemainingAlive);
		if (enemiesRemainingAlive == 0) {
			NextWave();
		}
	}

	void ResetPlayerPosition() {
		playerT.position = map.GetTileFromPosition (Vector3.zero).position + Vector3.up * 3;
	}

	IEnumerator waitForSeconds(float seconds){
		yield return new WaitForSeconds (seconds);	
	}

	void NextWave() {
		if (currentWaveNumber > 0) {
			
		}
		currentWaveNumber ++;

		StartCoroutine (waitForSeconds(2f));

		if (currentWaveNumber - 1 < waves.Length) {
			spawnedBoss = false;
			kingDeath = false;
			currentWave = waves [currentWaveNumber - 1];
			GameUIControl.instance.SetWaveText (currentWaveNumber);
			GameUIControl.instance.SetEnemyCountText (currentWave.enemyCount);
			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesRemainingAlive = enemiesRemainingToSpawn;
		} else {
			AudioManager.instance.PlaySound2D ("Level Complete");
			PlayerPrefsController.instance.SetXP (PlayerPrefsController.instance.GetXP ()+250);
			GameUIControl.instance.AnimateXpText ();  
			if (PlayerPrefsController.instance.GetQuestIndex () < 29) {
				PlayerPrefsController.instance.SetQuestIndex (PlayerPrefsController.instance.GetQuestIndex () + 1);
			} else {
				PlayerPrefsController.instance.SetDone (1);
			}
			StartCoroutine (WinEffect());
			player.StopWalking ();
		}
	}
	IEnumerator WinEffect(){
		
		if (PlayerPrefsController.instance.GetXP () % 1000 == 0){
			GameUIControl.instance.AnimateTalkPanel ("You are level "+((int)(PlayerPrefsController.instance.GetXP ()/1000) +1)+". Go meet me now!", 2);
			PlayerPrefsController.instance.SetUpgrade (PlayerPrefsController.instance.GetUpgrade()+10);
		} else {
			GameUIControl.instance.AnimateTalkPanel ("Good job! Let's go back home!",2);
		}
		yield return new WaitForSeconds (5.5f);
		GameUIControl.instance.GoHome ();
	}


	[System.Serializable]
	public class Wave {
		public bool infinite;
		public int enemyCount;
		public float timeBetweenSpawns;
		public Enemy[] enemy;
		public Enemy bossCEnemy;
		public Color[] skinColour;
		public Power[] power; 
	}

}

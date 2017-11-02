using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
	
	public Map[] maps;
	public int mapIndex;
	
	public Transform obstaclePrefab;
	public Transform navmeshFloor;
	public Transform navmeshMaskPrefab;
	public Transform obstacleMask;
	public Vector2 maxMapSize;
	
	[Range(0,1)]
	public float outlinePercent;
	
	public float tileSize;
	List<Coord> allTileCoords;
	Queue<Coord> shuffledTileCoords;
	Queue<Coord> shuffledOpenTileCoords;
	Transform[,] tileMap;
	
	Map currentMap;

	public Player[] player;
	Player spawnedPlayer;
	void Awake(){
		spawnedPlayer = Instantiate (player[PlayerPrefsController.instance.GetPlayer()], new Vector3(0,0,0f), Quaternion.identity) as Player;
		spawnedPlayer.playerMode = Player.PlayerMode.Gameplay;
		spawnedPlayer.startingHealth = PlayerPrefsController.instance.GetPlayerHealth ();
		spawnedPlayer.moveSpeed = PlayerPrefsController.instance.GetPlayerRunSpeed ();
		SetMapIndex (PlayerPrefsController.instance.GetQuestIndex());
		currentMap = maps [mapIndex];
		spawnedPlayer.OnDeath += OnPlayerDead;
	}

	public void SetMapIndex(int mapIndex){
		this.mapIndex = mapIndex;
		GenerateMap();
	}

	void OnPlayerDead(){
		StartCoroutine (AfterDieEffect());
	}

	IEnumerator AfterDieEffect(){
		GameUIControl.instance.AnimateTalkPanel ("Mission failed!", 1);
		yield return new WaitForSeconds (3.5f);
		GameUIControl.instance.GoHome ();
	}


	public void GenerateMap() {
		currentMap = maps[mapIndex];


		tileMap = new Transform[currentMap.mapSize.x,currentMap.mapSize.y];
		System.Random prng = new System.Random (currentMap.seed);

		// Generating coords
		allTileCoords = new List<Coord> ();
		for (int x = 0; x < currentMap.mapSize.x; x ++) {
			for (int y = 0; y < currentMap.mapSize.y; y ++) {
				allTileCoords.Add(new Coord(x,y));
			}
		}
		shuffledTileCoords = new Queue<Coord> (Utility.ShuffleArray (allTileCoords.ToArray (), currentMap.seed));

		// Create map holder object
		string holderName = "Generated Map";
		if (transform.FindChild (holderName)) {
			DestroyImmediate (transform.FindChild (holderName).gameObject);
		}
		
		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;


		// Generate Spawner
		Spawner newSpawner = Instantiate(currentMap.spawner, Vector3.zero, Quaternion.identity) as Spawner;
		newSpawner.transform.parent = mapHolder;

		// Spawning tiles
		for (int x = 0; x < currentMap.mapSize.x; x ++) {
			for (int y = 0; y < currentMap.mapSize.y; y ++) {
				Vector3 tilePosition = CoordToPosition(x,y);
				int randomIndex = prng.Next (0, currentMap.tiles.Length);
				Transform newTile = Instantiate (currentMap.tiles[randomIndex], tilePosition - Vector3.up * (currentMap.tiles[randomIndex].GetComponent<BoxCollider>().size.y+0.5f), Quaternion.identity) as Transform;
				newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
				newTile.parent = mapHolder;
				tileMap[x,y] = newTile;

				Renderer tileRenderer = newTile.GetComponent<Renderer>();
				Material tileMaterial = new Material(tileRenderer.sharedMaterial);
				float colourPercent = (float)prng.NextDouble();
				tileMaterial.color = Color.Lerp(currentMap.foreTileColour,currentMap.backTileColour,colourPercent);
				tileRenderer.sharedMaterial = tileMaterial;
			}
		}

		// Spawning obstacles
		bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x,(int)currentMap.mapSize.y];
		
		int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent) - (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.spacePercent);
		int spaceCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.spacePercent) - obstacleCount;
		int currentObstacleCount = 0;
		List<Coord> allOpenCoords = new List<Coord> (allTileCoords);
		
		for (int i =0; i < obstacleCount; i ++) {
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x,randomCoord.y] = true;
			currentObstacleCount ++;

			if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
				Vector3 obstaclePosition = CoordToPosition(randomCoord.x,randomCoord.y);
				int randomIndex = prng.Next (0, currentMap.obstacles.Length);
				Transform newObstacle = Instantiate(currentMap.obstacles[randomIndex], obstaclePosition , Quaternion.identity) as Transform;
				newObstacle.parent = mapHolder;
				newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, tileSize, (1 - outlinePercent) * tileSize);

				allOpenCoords.Remove(randomCoord);
			}
			else {
				obstacleMap[randomCoord.x,randomCoord.y] = false;
				currentObstacleCount --;
			}
		}
			

		for (int i =0; i < spaceCount; i ++) {
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x,randomCoord.y] = true;
			currentObstacleCount ++;

			if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
				Vector3 obstaclePosition = CoordToPosition(randomCoord.x,randomCoord.y);
				int randomIndex = prng.Next (0, currentMap.obstacles.Length);
				Transform newObstacle = Instantiate(obstacleMask, obstaclePosition , Quaternion.identity) as Transform;
				newObstacle.parent = mapHolder;
				newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, tileSize, (1 - outlinePercent) * tileSize);

				GetTileFromPosition (obstaclePosition).gameObject.SetActive (false);

				allOpenCoords.Remove(randomCoord);
			}
			else {
				obstacleMap[randomCoord.x,randomCoord.y] = false;
				currentObstacleCount --;
			}
		}

		shuffledOpenTileCoords = new Queue<Coord> (Utility.ShuffleArray (allOpenCoords.ToArray (), (int)Random.Range(0,100)));

		// Creating navmesh mask
		Transform maskLeft = Instantiate (navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskLeft.parent = mapHolder;
		maskLeft.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;
		
		Transform maskRight = Instantiate (navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskRight.parent = mapHolder;
		maskRight.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;
		
		Transform maskTop = Instantiate (navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskTop.parent = mapHolder;
		maskTop.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y-currentMap.mapSize.y)/2f) * tileSize;
	
		Transform maskBottom = Instantiate (navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskBottom.parent = mapHolder;
		maskBottom.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y-currentMap.mapSize.y)/2f) * tileSize;
		
		navmeshFloor.localScale = new Vector3 (maxMapSize.x, maxMapSize.y) * tileSize;
//		mapFloor.localScale =  new Vector3 (currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
	}
	
	bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (currentMap.mapCentre);
		mapFlags [currentMap.mapCentre.x, currentMap.mapCentre.y] = true;
		
		int accessibleTileCount = 1;
		
		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			
			for (int x = -1; x <= 1; x ++) {
				for (int y = -1; y <= 1; y ++) {
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if (x == 0 || y == 0) {
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
							if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]) {
								mapFlags[neighbourX,neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX,neighbourY));
								accessibleTileCount ++;
							}
						}
					}
				}
			}
		}
		
		int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
	}
	
	Vector3 CoordToPosition(int x, int y) {
		return new Vector3 (-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
	}

	public Transform GetTileFromPosition(Vector3 position) {
		int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
		int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
		x = Mathf.Clamp (x, 0, tileMap.GetLength (0) -1);
		y = Mathf.Clamp (y, 0, tileMap.GetLength (1) -1);
		return tileMap [x, y];
	}
	
	public Coord GetRandomCoord() {
		Coord randomCoord = shuffledTileCoords.Dequeue ();
		shuffledTileCoords.Enqueue (randomCoord);
		return randomCoord;
	}

	public Transform GetRandomOpenTile() {
		Coord randomCoord = shuffledOpenTileCoords.Dequeue ();
		shuffledOpenTileCoords.Enqueue (randomCoord);
		return tileMap[randomCoord.x,randomCoord.y];
	}
	
	[System.Serializable]
	public struct Coord {
		public int x;
		public int y;
		
		public Coord(int _x, int _y) {
			x = _x;
			y = _y;
		}
		
		public static bool operator ==(Coord c1, Coord c2) {
			return c1.x == c2.x && c1.y == c2.y;
		}
		
		public static bool operator !=(Coord c1, Coord c2) {
			return !(c1 == c2);
		}
		
	}
	
	[System.Serializable]
	public class Map {
		
		public Coord mapSize;
		[Range(0,1)]
		public float obstaclePercent;
		[Range(0,1)]
		public float spacePercent;
		public int seed;

		public Transform[] obstacles;
		public Transform[] tiles;

		public Color foreTileColour;
		public Color backTileColour;

		public Spawner spawner;

		public Coord mapCentre {
			get {
				return new Coord(mapSize.x/2,mapSize.y/2);
			}
		}
		
	}
}

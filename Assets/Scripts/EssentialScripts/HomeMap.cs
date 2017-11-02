using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HomeMap : MonoBehaviour {

	public Map[] maps;

	public float tileSize;

	Map currentMap;

	public Player[] player;
	Player spawnedPlayer;
	void Awake(){
		spawnedPlayer = Instantiate (player[PlayerPrefsController.instance.GetPlayer()], new Vector3(0,0,-1.7f), Quaternion.identity) as Player;
		spawnedPlayer.playerMode = Player.PlayerMode.Home;
		GenerateMap ();
	}



	public void GenerateMap() {
		currentMap = maps[0];
		System.Random prng = new System.Random (currentMap.seed);

		// Create map holder object
		string holderName = "Generated Map";
		if (transform.FindChild (holderName)) {
			DestroyImmediate (transform.FindChild (holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;

		Transform spawnedHomeCenter =Instantiate (currentMap.homeCenter, Vector3.zero, Quaternion.identity) as Transform;
		spawnedHomeCenter.localScale = Vector3.one * tileSize;
		spawnedHomeCenter.parent = mapHolder;

		// Spawning tiles
		for (int x = 0; x < currentMap.mapSize.x; x ++) {
			for (int y = 0; y < currentMap.mapSize.y; y ++) {
				Vector3 tilePosition = CoordToPosition(x,y);
				int randomIndex = prng.Next (0, currentMap.tiles.Length);
				Transform newTile = Instantiate (currentMap.tiles[randomIndex], tilePosition - Vector3.up * (currentMap.tiles[randomIndex].GetComponent<BoxCollider>().size.y+0.5f), Quaternion.identity) as Transform;
				newTile.parent = mapHolder;
				newTile.localScale = Vector3.one * tileSize;

				Renderer tileRenderer = newTile.GetComponent<Renderer>();
				Material tileMaterial = new Material(tileRenderer.sharedMaterial);
				float colourPercent = (float)prng.NextDouble();
				tileMaterial.color = Color.Lerp(currentMap.foreTileColour,currentMap.backTileColour,colourPercent);
				tileRenderer.sharedMaterial = tileMaterial;
			}
		}

		// Spawning obstacles
		for (int x=0; x < currentMap.mapSize.x; x++){
			for (int y=0; y< currentMap.mapSize.y ; y++){
				if (x == 0|| y == 0 || x == currentMap.mapSize.x-1|| y == currentMap.mapSize.y-1){
					Vector3 obstaclePosition = CoordToPosition(x,y);
					int randomIndex = prng.Next (0, currentMap.obstacles.Length);
					Transform newObstacle = Instantiate(currentMap.obstacles[randomIndex], obstaclePosition , Quaternion.identity) as Transform;
					newObstacle.parent = mapHolder;
					newObstacle.localScale = Vector3.one * tileSize;
				}
			}
		}



	}

	Vector3 CoordToPosition(int x, int y) {
		return new Vector3 (-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
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
		public int seed;

		public Transform[] obstacles;
		public Transform[] tiles;
		public Transform homeCenter;

		public Color foreTileColour;
		public Color backTileColour;

		public Coord mapCentre {
			get {
				return new Coord(mapSize.x/2,mapSize.y/2);
			}
		}

	}
}

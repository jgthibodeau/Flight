using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ChunkManager : MonoBehaviour {
	public bool doUpdate;
	public bool resetChunks = false;

	public float chunkSize;
	public int chunksX, chunksZ;
	public Vector3 chunksCenter;
	public Vector2 zeroChunk;

	public TerrainComposer2.Int2 currentChunk = new TerrainComposer2.Int2(0, 0);
	public List<List<Chunk>> chunks;
	private List<Chunk> loadedChunks;
	private List<Chunk> unloadableChunks;

	public GameObject defaultChunk;
	public int chunkRadius;

	public int chunkObjectRadius;

	private TerrainComposer2.TC_TerrainArea terrainArea;

	public Transform player;

	public bool replaceWater;
	public GameObject water;
	public float seaLevel;

	// Use this for initialization
	void Start () {
		terrainArea = GetComponent<TerrainComposer2.TC_TerrainArea> ();

		currentChunk = new TerrainComposer2.Int2(0, 0);
		chunks = new List<List<Chunk>>();
		loadedChunks = new List<Chunk>();
		unloadableChunks = new List<Chunk> ();

		//convert terrain's into an indexable array
		int currentRow = 0;
		List<Chunk> currentList = new List<Chunk>();
		chunks.Add (currentList);
		Debug.Log (terrainArea.terrains.ToArray ().Length + " terrain tiles");
		foreach(TerrainComposer2.TCUnityTerrain terrain in terrainArea.terrains){
			
			foreach (Transform t in terrain.terrain.transform) {
				if (t.name == water.transform.name + "(Clone)") {
					GameObject.DestroyImmediate (t.gameObject);
				}
			}
			if (replaceWater) {
				GameObject newWater = GameObject.Instantiate (water);
				newWater.transform.SetParent (terrain.terrain.transform);
				newWater.transform.localPosition = new Vector3 (0, seaLevel, 0);
				newWater.SetActive (true);
			}

			if (terrain.tileZ != currentRow) {
				currentRow = terrain.tileZ;
				currentList = new List<Chunk>();
				chunks.Add (currentList);
			}

			currentList.Add (new Chunk(terrain));
			terrain.terrain.gameObject.SetActive (false);
		}
		Debug.Log (chunks.ToArray ().Length + " rows added to chunk list");

		chunksX = terrainArea.tiles.y;
		chunksZ = terrainArea.tiles.x;
		chunkSize = terrainArea.terrainSize.x;
		chunksCenter = terrainArea.center;
		zeroChunk = new Vector2 (chunksCenter.x - chunkSize/2f - (chunksX/2 * chunkSize), chunksCenter.z - chunkSize/2f - (chunksZ/2 * chunkSize));
	}
	
	// Update is called once per frame
	void Update () {
		if (resetChunks) {
			Start ();
			resetChunks = false;
		}

		if (doUpdate) {
			TerrainComposer2.Int2 newChunk = new TerrainComposer2.Int2 ((int)((player.position.x - zeroChunk.x) / chunkSize), (int)((player.position.z - zeroChunk.y) / chunkSize));
			//		Debug.Log ("********************");
			//		Debug.Log ("currentChunk: " + currentChunk.x + ", " + currentChunk.y);
			//		Debug.Log ("newChunk: " + newChunk.x + ", " + newChunk.y);
			if (currentChunk.x != newChunk.x || currentChunk.y != newChunk.y) {
				currentChunk = newChunk;
				UpdateChunks ();
			}
		}
	}

	void UpdateChunks(){
		int minChunkX = currentChunk.x - chunkRadius;
		int maxChunkX = currentChunk.x + chunkRadius;
		int minChunkZ = currentChunk.y - chunkRadius;
		int maxChunkZ = currentChunk.y + chunkRadius;


		int minChunkObjectX = currentChunk.x - chunkObjectRadius;
		int maxChunkObjectX = currentChunk.x + chunkObjectRadius;
		int minChunkObjectZ = currentChunk.y - chunkObjectRadius;
		int maxChunkObjectZ = currentChunk.y + chunkObjectRadius;

		//set unloadable chunks equal to currently loaded chunks
		unloadableChunks.Clear ();
		foreach (Chunk chunk in loadedChunks) {
			unloadableChunks.Add (chunk);
		}

		for (int x = minChunkX; x <= maxChunkX; x++) {
			for (int z = minChunkZ; z <= maxChunkZ; z++) {
				//if requested chunk is non-existant
//				Debug.Log (x + ", " + z);
				if (x < 0 || x >= chunks.ToArray ().Length || z < 0 || z >= chunks [0].ToArray ().Length) {
					//TODO spawn in a default chunk
//					Debug.Log ("out of range");
				} else {
					Chunk chunk = chunks [z] [x];

					//put chunk into list of currently loaded chunks and enable it
					if (!loadedChunks.Contains (chunk)) {
						chunk.toggleTerrain (true);
						loadedChunks.Add (chunk);
					}

					//remove chunk from the list of unloadable chunks
					if (unloadableChunks.Contains (chunk)) {
						unloadableChunks.Remove (chunk);
					}

					//if chunk is within object distance, enable its objects
					if (x >= minChunkObjectX && x <= maxChunkObjectX && z >= minChunkObjectZ && z <= maxChunkObjectZ) {
						chunk.toggleObjects (true);
					}
				}
			}
		}

		//disable all chunks in the list of unloadable chunks and remove them from the loaded chunks list
		foreach (Chunk chunk in unloadableChunks) {
			chunk.toggleTerrain (false);
			chunk.toggleObjects (false);
			loadedChunks.Remove (chunk);
		}
	}

	public class Chunk{
		private TerrainComposer2.TCUnityTerrain tc_terrain;
		private GameObject objectManager;
		private bool objectsEnabled = false;
		private bool terrainEnabled = false;

		public Chunk(TerrainComposer2.TCUnityTerrain tc_terrain){
			this.tc_terrain = tc_terrain;
		}

		public void toggleTerrain(bool enabled) {
			if (enabled != terrainEnabled) {
				tc_terrain.terrain.gameObject.SetActive (enabled);
				terrainEnabled = enabled;
			}
		}

		public void toggleObjects(bool enabled) {
			if (enabled != objectsEnabled) {
				objectManager.SetActive (enabled);
				objectsEnabled = enabled;
			}
		}
	}
}

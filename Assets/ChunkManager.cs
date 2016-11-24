using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ChunkManager : MonoBehaviour {
	public bool resetChunks = false;

	public float chunkSize;
	public int chunksX, chunksZ;
	public Vector3 chunksCenter;
	public Vector2 zeroChunk;
	public TerrainComposer2.Int2 currentChunk = new TerrainComposer2.Int2(0, 0);

	public List<List<Chunk>> chunks = new List<List<Chunk>>();

	private List<Chunk> loadedChunks = new List<Chunk>();
	private List<Chunk> unloadableChunks = new List<Chunk> ();

	public GameObject defaultChunk;
	public int chunkRadius;

	private TerrainComposer2.TC_TerrainArea terrainArea;

	public Transform player;

	// Use this for initialization
	void Start () {
		terrainArea = GetComponent<TerrainComposer2.TC_TerrainArea> ();
		//convert terrain's into an indexable array
		int currentRow = 0;
		List<Chunk> currentList = new List<Chunk>();
		chunks.Add (currentList);
		Debug.Log (terrainArea.terrains.ToArray ().Length + " terrain tiles");
		foreach(TerrainComposer2.TCUnityTerrain terrain in terrainArea.terrains){
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

		currentChunk = new TerrainComposer2.Int2 (0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (resetChunks) {
			Start ();
			resetChunks = false;
		}

		TerrainComposer2.Int2 newChunk = new TerrainComposer2.Int2((int)((player.position.x - zeroChunk.x) / chunkSize), (int)((player.position.z - zeroChunk.y) / chunkSize));
//		Debug.Log ("********************");
//		Debug.Log ("currentChunk: " + currentChunk.x + ", " + currentChunk.y);
//		Debug.Log ("newChunk: " + newChunk.x + ", " + newChunk.y);
		if (currentChunk.x != newChunk.x || currentChunk.y != newChunk.y) {
			currentChunk = newChunk;
			UpdateChunks ();
		}
	}

	void UpdateChunks(){
		int minChunkX = currentChunk.x - chunkRadius;
		int maxChunkX = currentChunk.x + chunkRadius;
		int minChunkZ = currentChunk.y - chunkRadius;
		int maxChunkZ = currentChunk.y + chunkRadius;
//		Debug.Log ("chunkX range: " + minChunkX + " - " + maxChunkX);
//		Debug.Log ("chunkZ range: " + minChunkZ + " - " + maxChunkZ);

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
						chunk.tc_terrain.terrain.gameObject.SetActive (true);
						loadedChunks.Add (chunk);
					}

					//remove chunk from the list of unloadable chunks
					if (unloadableChunks.Contains (chunk)) {
						unloadableChunks.Remove (chunk);
					}
				}
			}
		}

		//disable all chunks in the list of unloadable chunks and remove them from the loaded chunks list
		foreach (Chunk chunk in unloadableChunks) {
			chunk.tc_terrain.terrain.gameObject.SetActive (false);
			loadedChunks.Remove (chunk);
		}
	}

	public class Chunk{
		public TerrainComposer2.TCUnityTerrain tc_terrain;

		public Chunk(TerrainComposer2.TCUnityTerrain tc_terrain){
			this.tc_terrain = tc_terrain;
		}
	}
}

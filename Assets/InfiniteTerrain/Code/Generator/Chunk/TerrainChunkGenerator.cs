using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerrainGenerator
{
    public class TerrainChunkGenerator : MonoBehaviour
    {
        public Material TerrainMaterial;

        public Texture2D FlatTexture;
        public Texture2D SteepTexture;

		public int terrainHeightmapResolution = 257;
		public int terrainAlphamapResolution = 257;
		public int terrainLength = 1000;
		public int terrainHeight = 800;
		public float seaLevel = 0.1f;

		public int numberTrees = 5000;
		public int billboardStart = 500;
		public int maxFullLODTrees = 16;
		public GameObject[] trees;
		public float[] treePercents;

        private TerrainChunkSettings Settings;

//		public int numberNoiseProviders = 1;
		private NoiseProvider[] NoiseProvider;
		public double[] frequency; //4.0
		public double[] lacunarity; //2.0
		public int[] seed;
		public bool randomSeed;

        private ChunkCache Cache;

        private void Awake()
        {
			Settings = new TerrainChunkSettings(terrainHeightmapResolution, terrainAlphamapResolution, terrainLength, terrainHeight, seaLevel,
				FlatTexture, SteepTexture, TerrainMaterial, numberTrees, billboardStart,
				maxFullLODTrees, trees, treePercents);
			NoiseProvider = new NoiseProvider[frequency.Length];
			for (int i = 0; i < NoiseProvider.Length; i++) {
				if (randomSeed)
					NoiseProvider[i] = new NoiseProvider (frequency[i], lacunarity[i]);
				else
					NoiseProvider[i] = new NoiseProvider (frequency[i], lacunarity[i], seed[i]);
			}

            Cache = new ChunkCache();
        }

        private void Update()
        {
            Cache.Update();
        }

        private void GenerateChunk(int x, int z)
        {
            if (Cache.ChunkCanBeAdded(x, z))
            {
                var chunk = new TerrainChunk(Settings, NoiseProvider, x, z);
                Cache.AddNewChunk(chunk);
            }
        }

        private void RemoveChunk(int x, int z)
        {
            if (Cache.ChunkCanBeRemoved(x, z))
                Cache.RemoveChunk(x, z);
        }

        private List<Vector2i> GetChunkPositionsInRadius(Vector2i chunkPosition, int radius)
        {
            var result = new List<Vector2i>();

            for (var zCircle = -radius; zCircle <= radius; zCircle++)
            {
                for (var xCircle = -radius; xCircle <= radius; xCircle++)
                {
                    if (xCircle * xCircle + zCircle * zCircle < radius * radius)
                        result.Add(new Vector2i(chunkPosition.X + xCircle, chunkPosition.Z + zCircle));
                }
            }

            return result;
        }

		public IEnumerator UpdateTerrain(Vector3 worldPosition, int radius, int chunksPerFrame, int yieldTimes)
        {
            var chunkPosition = GetChunkPosition(worldPosition);
			var newPositions = GetChunkPositionsInRadius(chunkPosition, radius);

            var loadedChunks = Cache.GetGeneratedChunks();
			var chunksToRemove = loadedChunks.Except(newPositions).ToList();

			var positionsToGenerate = newPositions.Except(chunksToRemove).ToList();

			foreach (var position in positionsToGenerate) {
				GenerateChunk (position.X, position.Z);
				for(int i=0;i<yieldTimes;i++)
					yield return null;
			}

			foreach (var position in chunksToRemove) {
				RemoveChunk (position.X, position.Z);
				for(int i=0;i<yieldTimes;i++)
					yield return null;
			}
        }

        public Vector2i GetChunkPosition(Vector3 worldPosition)
        {
            var x = (int)Mathf.Floor(worldPosition.x / Settings.Length);
            var z = (int)Mathf.Floor(worldPosition.z / Settings.Length);

            return new Vector2i(x, z);
        }

        public bool IsTerrainAvailable(Vector3 worldPosition)
        {
            var chunkPosition = GetChunkPosition(worldPosition);
            return Cache.IsChunkGenerated(chunkPosition);
        }

        public float GetTerrainHeight(Vector3 worldPosition)
        {
            var chunkPosition = GetChunkPosition(worldPosition);
            var chunk = Cache.GetGeneratedChunk(chunkPosition);
            if (chunkPosition != null)
                return chunk.GetTerrainHeight(worldPosition);

            return 0;
        }
    }
}
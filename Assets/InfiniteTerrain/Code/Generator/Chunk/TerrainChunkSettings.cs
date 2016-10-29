using UnityEngine;

namespace TerrainGenerator
{
    public class TerrainChunkSettings
    {
        public int HeightmapResolution { get; private set; }

        public int AlphamapResolution { get; private set; }

        public int Length { get; private set; }

        public int Height { get; private set; }

		public int SeaLevel { get; private set; }

        public Texture2D FlatTexture { get; private set; }

        public Texture2D SteepTexture { get; private set; }

		public Material TerrainMaterial { get; private set; }

		public int NumberTrees { get; private set; }

		public int BillboardStart { get; private set; }

		public int MaxFullLODTrees { get; private set; }

		public GameObject[] Trees { get; private set; }

		public float[] TreePercents { get; private set; }

		public TerrainChunkSettings(int heightmapResolution, int alphamapResolution, int length, int height, int seaLevel,
			Texture2D flatTexture, Texture2D steepTexture, Material terrainMaterial, int numberTrees, int billboardStart,
			int maxFullLODTrees, GameObject[] trees, float[] treePercents)
        {
            HeightmapResolution = heightmapResolution;
            AlphamapResolution = alphamapResolution;
            Length = length;
            Height = height;
			SeaLevel = seaLevel;
            FlatTexture = flatTexture;
            SteepTexture = steepTexture;
            TerrainMaterial = terrainMaterial;
			NumberTrees = numberTrees;
			BillboardStart = billboardStart;
			MaxFullLODTrees = maxFullLODTrees;
			Trees = trees;
			TreePercents = treePercents;
        }
    }
}
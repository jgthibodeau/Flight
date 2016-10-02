using LibNoise.Generator;

namespace TerrainGenerator
{
    public class NoiseProvider : INoiseProvider
    {
        private Perlin PerlinNoiseGenerator;

        public NoiseProvider()
        {
            PerlinNoiseGenerator = new Perlin();
			PerlinNoiseGenerator.Frequency = 4.0;
			PerlinNoiseGenerator.Lacunarity = 2.0;
			PerlinNoiseGenerator.Persistence = 0.5;

//			private double _frequency = 1.0;
//			private double _lacunarity = 2.0;
//			private QualityMode _quality = QualityMode.Medium;
//			private int _octaveCount = 6;
//			private double _persistence = 0.5;
        }

        public float GetValue(float x, float z)
        {
            return (float)(PerlinNoiseGenerator.GetValue(x, 0, z) / 2f) + 0.5f;
        }
    }
}
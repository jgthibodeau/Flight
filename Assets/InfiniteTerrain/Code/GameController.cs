using System.Collections;
using TerrainGenerator;
using UnityEngine;
using System.Threading;

public class GameController : MonoBehaviour
{
    public int Radius = 4;

    private Vector2i PreviousPlayerChunkPosition;

    public Transform Player;

    public TerrainChunkGenerator Generator;

	public int ChunksPerFrame;

	public int YieldTimes;

    public void Start()
    {
		StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        var canActivateCharacter = false;

		yield return Generator.UpdateTerrain(Player.position, Radius, ChunksPerFrame, 0);

        do
        {
            var exists = Generator.IsTerrainAvailable(Player.position);
            if (exists)
                canActivateCharacter = true;
            yield return null;
        } while (!canActivateCharacter);

        PreviousPlayerChunkPosition = Generator.GetChunkPosition(Player.position);
		float minY = Generator.GetTerrainHeight (Player.position) + 0.5f;
		if (Player.position.y < minY) {
			Player.position = new Vector3 (Player.position.x, minY, Player.position.z);
		}
        Player.gameObject.SetActive(true);

		StartCoroutine(UpdateCoroutine());
    }

	private IEnumerator UpdateCoroutine()
    {
		while (true) {
			if (Player.gameObject.activeSelf) {
				var playerChunkPosition = Generator.GetChunkPosition (Player.position);
				if (!playerChunkPosition.Equals (PreviousPlayerChunkPosition)) {
					yield return Generator.UpdateTerrain (Player.position, Radius, ChunksPerFrame, YieldTimes);
					PreviousPlayerChunkPosition = playerChunkPosition;
				}
			}
			yield return null;
		}
    }
}
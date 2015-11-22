using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnBall : MonoBehaviour {
	public GameObject ballPrefab;
	private bool spawnedYet = false;
	public bool SpawnedYet { get; set; }
}

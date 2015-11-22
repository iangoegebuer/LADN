using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Squat : NetworkBehaviour {
	[SyncVar]
	float r, g, b;

	[SyncVar]
	public float rotation;

	public float lifeSpan;
	public float lifeSpanRange;

	// Use this for initialization
	protected virtual void Start () {
		if (isServer) {
			r = Random.value;
			g = Random.value;
			b = Random.value;
		}

		StartCoroutine(KillMeIn(lifeSpan + lifeSpanRange * Random.value));
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));
		GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, new Color(r, g, b), 0.7f);
	}

	IEnumerator KillMeIn (float time) {
		yield return new WaitForSeconds(time);
		if (isServer) {
			NetworkServer.UnSpawn(gameObject);
		}
		Destroy (gameObject);
	}
}

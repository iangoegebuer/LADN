﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Squat : NetworkBehaviour {
	[SyncVar]
	float r, g, b;

	public float lifeSpan;
	public float lifeSpanRange;

	// Use this for initialization
	void Start () {
		if (isServer) {
			r = Random.value;
			g = Random.value;
			b = Random.value;
		}

		StartCoroutine(KillMeIn(lifeSpan + lifeSpanRange * Random.value));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator KillMeIn (float time) {
		yield return new WaitForSeconds(time);
		if (isServer) {
			NetworkServer.UnSpawn(gameObject);
		}
		Destroy (gameObject);
	}
}

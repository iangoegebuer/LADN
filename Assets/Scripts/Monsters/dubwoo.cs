using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class dubwoo : Squat {
	public GameObject ballPrefab;
	public float ballExpires;
	public float waitToPoop;
	protected override void Start () {
		base.Start();
		if (isServer) {
			//StartCoroutine(PoopBall());
		}
	}
	
	protected override void Update () {
		base.Update();
	}
	
	void OnTriggerEnter(Collider other) {
		if (isServer) {
			if(other.GetComponent<BallCtl>() != null) {
				Debug.Log ("Collide with ball");
				other.GetComponent<BallCtl>().Accelerate();
			}
		}
	}
}

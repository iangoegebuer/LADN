using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SquatExt : Squat {
	public GameObject ballPrefab;
	public float ballExpires;
	public float waitToPoop;
	protected override void Start () {
		base.Start();
		if (isServer) {
			StartCoroutine(PoopBall());
		}
	}

	protected override void Update () {
		base.Update();
	}

	IEnumerator PoopBall () {
		yield return new WaitForSeconds(waitToPoop);
		GameObject ball = Instantiate<GameObject>(ballPrefab);
		ball.GetComponent<BallCtl>().scores = GameObject.Find("Scoreboard 1 (Clone)");
		ball.GetComponent<BallCtl>().expires = ballExpires;
		ball.transform.position = transform.position + transform.up * 6f;
		Debug.Log("poop " + ball.GetComponent<BallCtl>().scores);
		NetworkServer.Spawn(ball);
	}
}

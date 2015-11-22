using UnityEngine;
using System.Collections;

public class BallCtl : MonoBehaviour {
	public GameObject scores;
	Rigidbody rb;
	Vector3 startPos;

	// Use this for initialization  ((Random.value - 0.5f)<0)?1f:
	void Start () {
		rb = GetComponent<Rigidbody>();
		startPos = transform.position;
		rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, ((Random.value - 0.5f)<0)?1f:-1f) * 50f;
	}

	static IEnumerator ResetTrail(TrailRenderer trail)
	{
		var trailTime = trail.time;
		trail.time = -1;
		yield return new WaitForEndOfFrame();
		trail.time = trailTime;
	}  
	
	// Update is called once per frame
	void Update () {
		if (transform.position.z < -25f) {
			transform.position = startPos;
			rb = GetComponent<Rigidbody>();
			
			rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, 1f) * 50f;
			scores.GetComponent<ScoreHandler>().UpdateRed(1);
			TrailRenderer TR = GetComponent<TrailRenderer>();
			ResetTrail(TR);
			
		}
		if (transform.position.z > 180f) {
			transform.position = startPos;
			rb = GetComponent<Rigidbody>();
			
			rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, -1f) * 50f;
			scores.GetComponent<ScoreHandler>().UpdateGreen(1);
			TrailRenderer TR = GetComponent<TrailRenderer>();
			ResetTrail(TR);
		}
	}
}

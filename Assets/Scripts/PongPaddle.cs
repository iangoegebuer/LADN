using UnityEngine;
using System.Collections;

public class PongPaddle : MonoBehaviour {
	struct PaddlePosition {
		public float x;
		public float y;
	}

	public GameObject cam;
	public GameObject collide;
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;


	PaddlePosition pos;
	float z = 0f;
	// Use this for initialization
	void Start () {
		pos.x = 0f;
		pos.y = 0f;
		z = transform.position.z;
		transform.position.Set (pos.x, pos.y, z);
	}
	
	// Update is called once per frame
	void Update () {

		RaycastHit[] hits;
		hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 100.0F);

		//Debug.Log (hits.Length);

		for (int i = 0; i < hits.Length; i++) {
			RaycastHit hit = hits [i];

			if(hit.collider.gameObject == collide) {
				if(hit.point.x > minX && hit.point.x < maxX)
					pos.x = hit.point.x;
				else if( hit.point.x < minX)
					pos.x = minX;
				else pos.x = maxX;
				if(hit.point.y > minY && hit.point.y < maxY)
					pos.y = hit.point.y;
				else if( hit.point.y < minY)
					pos.y = minY;
				else pos.y = maxY;
				Debug.Log(hit.collider.name + " X: " + pos.x + " Y: " + pos.y);
				z = GetComponent<Renderer> ().transform.position.z;
				Vector3 vec = new Vector3 (pos.x, pos.y, z);
				transform.position = vec;
			}

		}


	}
}

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ScoreHandler : NetworkBehaviour {
	public Text RedScore;
	public Text GreenScore;

	[SyncVar]
	int redScore,greenScore;

	// Use this for initialization
	void Start () {
		if (isServer) {
			redScore = 0;
			greenScore = 0;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		GreenScore.text = greenScore.ToString ("D2");
		RedScore.text = redScore.ToString ("D2");
	}


	public void UpdateRed(int score) {
		if (isServer) {
			redScore += score;
			RedScore.text = redScore.ToString ("D2");
		}
	}
	
	public void UpdateGreen(int score) {
		if (isServer) {
			greenScore += score;
			GreenScore.text = greenScore.ToString ("D2");
		}
	}

}

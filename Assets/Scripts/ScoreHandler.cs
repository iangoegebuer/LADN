using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour {
	public Text RedScore;
	public Text GreenScore;
	int redScore;
	int greenScore;

	// Use this for initialization
	void Start () {
		redScore = 0;
		greenScore = 0;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void UpdateRed(int score) {
		redScore += score;
		RedScore.text = redScore.ToString ("D2");
	}
	
	public void UpdateGreen(int score) {
		greenScore += score;
		GreenScore.text = greenScore.ToString ("D2");
	}

}

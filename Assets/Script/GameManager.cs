using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public enum GamingState{
	GameLoding,
	Playing,
	CalculateResult,
	ResultScene
}

public enum PlayingState {
	Null,
	ObjCreate,
	BonusLevel,
	ChoiceTime,
	LateForChoice,
	ChoiceMoving,
	Result
}

public enum RecoverChoice{
	Null,
	Left,
	Right
}



public class MedicineInfo{
	public string name;
	public int AppearedID;
}



public class GameManager : MonoBehaviour {


	[System.Serializable]
	public class MedicineInfo{
		public string name;
		public int appearedID;
	}


	public GamingState gamingState;
	public PlayingState playingState;
	public MedicineInfo[] medicine = new MedicineInfo[5];
	public Animator anim;
	public int TimePerRound = 45;
	public float playingSpeed = 1;

	private RecoverChoice recoverChoice;
	private List<int> numberForID;
	private int curID;
	private MedicineInfo curMedicine;


	void Start () {
		anim = this.gameObject.GetComponent<Animator>();
		gamingState = GamingState.GameLoding;
		playingState = PlayingState.Null;
		GamingAnimControl();
	}


	void Update () {
	
	}



	#region Gaming Function
	public void InitializationScene () {
		Debug.Log("Initialization Scene ,wait for Start Game");
	}

	private void StartCalculateTime () {
		StartCoroutine(CalculateTime());
		TimeUp();
	}

	private IEnumerator CalculateTime () {
		yield return new WaitForSeconds(TimePerRound);
	}

	private void TimeUp () {
		gamingState = GamingState.CalculateResult;
		GamingAnimControl();
	}

	public void CalculateResult () {
		Debug.Log("Play CalculateResult Animation");
		gamingState = GamingState.ResultScene;
		GamingAnimControl();
	}

	public void ResultScene () {
		Debug.Log("Show ResultScene, wait for InitializationScene");
		gamingState = GamingState.GameLoding;
		GamingAnimControl();
	}


	private void GamingAnimControl () {
		switch(gamingState){
		case GamingState.GameLoding:
			InitializationScene();
			break;
		case GamingState.Playing:
			StartCalculateTime();
			PlayingGame();
			break;
		case GamingState.CalculateResult:
			CalculateResult();
			break;
		case GamingState.ResultScene:
			ResultScene();
			break;
		}
	}
	#endregion


	#region Playing Fumction
	private void PlayingGame () {
		StartCoroutine(MedicineObjProcess());
		if(gamingState == GamingState.Playing){
			PlayingGame();
		}
	}

	private IEnumerator MedicineObjProcess () {
		if(medicine[0].appearedID == 0){
			ResetAppearedID();
		}
		while(playingState != PlayingState.Null){
			yield return null;
		}

		foreach(var m in medicine){
			if(m.appearedID == curID){
				curMedicine = m;
				playingState = PlayingState.ObjCreate;
			}
		}
		PlayingAnimControl();
		while(playingState == PlayingState.ObjCreate){
			yield return null;
		}
		if(playingState == PlayingState.BonusLevel){
			PlayingAnimControl();
			while(playingState == PlayingState.BonusLevel){
				yield return null;
			}
		}
		PlayingAnimControl();
		while(playingState == PlayingState.ChoiceTime){
			yield return null;
		}
		if(recoverChoice == RecoverChoice.Null){
			playingState = PlayingState.LateForChoice;
		}
		PlayingAnimControl();
		while(playingState != PlayingState.Result){
			yield return null;
		}
		PlayingAnimControl();

		if(curID != 5){
			curID +=1;
		}else if(curID == 5){
			curID = 1;
			ResetAppearedID();
		}
		playingState = PlayingState.Null;
	}

	private void PlayingAnimControl(){
		switch(playingState){
		case PlayingState.ObjCreate:
			break;
		case PlayingState.BonusLevel:
			break;
		case PlayingState.ChoiceTime:
			break;
		case PlayingState.LateForChoice:
			break;
		case PlayingState.ChoiceMoving:
			break;
		case PlayingState.Result:
			break;
		}
	}

	private void ResetAppearedID () {
		numberForID.Add(1);
		numberForID.Add(2);
		numberForID.Add(3);
		numberForID.Add(4);
		numberForID.Add(5);

		foreach(var m1 in medicine){
			float a = Random.Range(0f,(float)numberForID.Count-1f);
			m1.appearedID = numberForID[(int)a];
			numberForID.Remove(numberForID[(int)a]);
		}
	}

	#endregion



	public void GameStart () {
		gamingState = GamingState.Playing;
		GamingAnimControl();
	}

	public void ReachLocation () {
		if(curMedicine.name == "AA"){
			playingState = PlayingState.BonusLevel;
		}else{
			playingState = PlayingState.ChoiceTime;
		}
	}

}

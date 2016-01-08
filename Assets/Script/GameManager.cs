using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;




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
public enum Result{
	Null,
	Miss,
	Wrong,
	Correct
}


public class MedicineInfo{
	public string name;
	public int AppearedID;
}



public class GameManager : MonoBehaviour {


	[System.Serializable]
	public class MedicineInfo{
		public string name;
		public int number;
		public int appearedID;
	}


	public GamingState gamingState;
	public PlayingState playingState;
	public MedicineInfo[] medicine = new MedicineInfo[5];
	public Animator anim;
	public Animator animTimer;
	public float TimePerRound = 45f;
	public float timeData = 0f;
	public float playingSpeed = 1;
	public int score = 0;
	public GameObject[] medicineObj = new GameObject[5];
	public List<int> numberForID;
	public Text scoreTxt;
	public Text timeTxt;


	public Result choiceResult;
	private RecoverChoice recoverChoice;
	public int curID = 1;
	private bool isTimer = false;
	private bool isPlayingGame = false;


	public MedicineInfo curMedicine;







	void Start () {
		anim = this.gameObject.GetComponent<Animator>();
		gamingState = GamingState.GameLoding;
		playingState = PlayingState.Null;
		GamingAnimControl();
	}


	void Update () {
		anim.speed = playingSpeed;
		animTimer.speed = playingSpeed;
		anim.SetInteger("curMedicine",curMedicine.number);
		if(isTimer == true){
			StartCalculateTime();
		}
		if(isPlayingGame == true){
			StartCoroutine(MedicineObjProcess());
			isPlayingGame = false;
		}

		if(playingState == PlayingState.ChoiceTime){
			if(Input.GetKey(KeyCode.LeftArrow)){
				playingState = PlayingState.ChoiceMoving;
				anim.SetTrigger("chooseLeft");
				animTimer.SetTrigger("OnLeft");
				SelectionJudge(true);
			}else if(Input.GetKey(KeyCode.RightArrow)){
				playingState = PlayingState.ChoiceMoving;
				anim.SetTrigger("chooseRight");
				animTimer.SetTrigger("OnRight");
				SelectionJudge(false);
			}
		}

		scoreTxt.text = score.ToString();
		timeTxt.text = (45-(int)timeData).ToString();

	}











	#region Gaming Function
	public void InitializationScene () {
		
	}

	public void GameStart () {
		gamingState = GamingState.Playing;
		GamingAnimControl();
	}

	private void StartCalculateTime () {
		timeData += Time.deltaTime;
		if(timeData >= TimePerRound){
			TimeUp();
		}
	}

	private void TimeUp () {
		gamingState = GamingState.CalculateResult;
		isTimer = false;
		GamingAnimControl();
	}

	public void CalculateResult () {
		gamingState = GamingState.ResultScene;
		GamingAnimControl();
	}

	public void ResultScene () {
		gamingState = GamingState.GameLoding;
		GamingAnimControl();
	}


	private void GamingAnimControl () {
		switch(gamingState){
		case GamingState.GameLoding:
			InitializationScene();
			break;
		case GamingState.Playing:
			isTimer = true;
			timeData = 0;
			isPlayingGame = true;
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
	public void ReachLocation () {
		if(curMedicine.name == "medicine5"){
			playingState = PlayingState.BonusLevel;
		}else{
			playingState = PlayingState.ChoiceTime;
		}
	}

	public void WaitTimeUp () {
		playingState = PlayingState.LateForChoice;
	}

	public void GoResult () {
		playingState = PlayingState.Result;
	}


	private void SelectionJudge(bool _isChooseLeft) {
		if(_isChooseLeft == false){
			if(curMedicine.number == 1 || curMedicine.number == 2 ||curMedicine.number == 4 ||curMedicine.number == 5){
				choiceResult = Result.Correct;
			}else if(curMedicine.number == 3){
				choiceResult = Result.Wrong;
			}
		}else if(_isChooseLeft == true){
			if(curMedicine.number == 1 || curMedicine.number == 2 ||curMedicine.number == 4 ||curMedicine.number == 5){
				choiceResult = Result.Wrong;
			}else if(curMedicine.number == 3){
				choiceResult = Result.Correct;
			}
		}
	}

	public void HideSprite(){
		medicineObj[0].SetActive(false);
		medicineObj[1].SetActive(false);
		medicineObj[2].SetActive(false);
		medicineObj[3].SetActive(false);
		medicineObj[4].SetActive(false);
	}

	private IEnumerator MedicineObjProcess () {
		if(medicine[0].appearedID == 0){
			ResetAppearedID();
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
		choiceResult = Result.Null;
		PlayingAnimControl();
		while(playingState == PlayingState.ChoiceTime){
			yield return null;
		}
		PlayingAnimControl();
		while(playingState != PlayingState.Result){
			yield return null;
		}
		PlayingAnimControl();

		Debug.Log(curID);
		if(curID != 5){
			curID +=1;
		}else if(curID == 5){
			curID = 1;
			ResetAppearedID();
		}
		playingState = PlayingState.Null;
		if(gamingState == GamingState.Playing){
			isPlayingGame = true;
		}
	}

	private void PlayingAnimControl(){
		switch(playingState){
		case PlayingState.ObjCreate:
			foreach(var o in medicine){
				if(o.name == curMedicine.name){
					switch(o.number){
					case 1:
						medicineObj[0].SetActive(true);
						break;
					case 2:
						medicineObj[1].SetActive(true);
						break;
					case 3:
						medicineObj[2].SetActive(true);
						break;
					case 4:
						medicineObj[3].SetActive(true);
						break;
					case 5:
						medicineObj[4].SetActive(true);
						break;
					}
				}else{
					switch(o.number){
					case 1:
						medicineObj[0].SetActive(false);
						break;
					case 2:
						medicineObj[1].SetActive(false);
						break;
					case 3:
						medicineObj[2].SetActive(false);
						break;
					case 4:
						medicineObj[3].SetActive(false);
						break;
					case 5:
						medicineObj[4].SetActive(false);
						break;
					}
				}
			}
			anim.SetTrigger("reStart");
			break;

		case PlayingState.BonusLevel:
			break;

		case PlayingState.ChoiceTime:
			animTimer.SetTrigger("StartTimer");
			break;

		case PlayingState.LateForChoice:
			anim.SetTrigger("miss");
			choiceResult = Result.Miss;
			break;

		case PlayingState.ChoiceMoving:
			break;

		case PlayingState.Result:
			if(gamingState == GamingState.Playing){
				switch(choiceResult){
				case Result.Miss:
					score -= 10 ; 
					break;
				case Result.Wrong:
					score -= 15;
					break;
				case Result.Correct:
					score += 10;
					break;
				}
			}
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




}

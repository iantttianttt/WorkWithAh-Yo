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
	public Animator animBonus;

	public GameObject[] medicineObj = new GameObject[5];
	public List<int> numberForID;
	public Text scoreTxt;
	public Text timeTxt;
	public GameObject bonusLevelObj;
	public Sprite[] bonusLevelSprite = new Sprite[9];
	public MedicineInfo curMedicine;
	public AudioSource bgm;
	public AudioSource crrectSound;
	public AudioSource wrongSound;
	public AudioSource timeUpSound;
	public float timePerRound = 45f;
	public float curTime = 0f;
	public float playingSpeed = 1;
	public int score = 0;
	public int crrectPoint;
	public int wrongPoint;
	public int missPoint;
	public int bonusPoint;



	private Result choiceResult;
	private RecoverChoice recoverChoice;
	private int curID = 1;
	private bool isTimer = false;
	private bool isPlayingGame = false;
	private SpriteRenderer curSprite;
	private bool canChooose = true;









	void Start () {
		anim = this.gameObject.GetComponent<Animator>();
		curSprite = bonusLevelObj.GetComponent<SpriteRenderer>();
		animBonus = GameObject.Find("BonusLevel").GetComponent<Animator>();
		gamingState = GamingState.GameLoding;
		playingState = PlayingState.Null;
		GamingAnimControl();
	}
		
	void Update () {
		anim.speed = playingSpeed;
		animTimer.speed = playingSpeed;
		animBonus.speed = playingSpeed;
		anim.SetInteger("curMedicine",curMedicine.number);
		if(isTimer == true){
			StartCalculateTime();
		}
		if(isPlayingGame == true){
			StartCoroutine(MedicineObjProcess());
			isPlayingGame = false;
		}

		if(playingState == PlayingState.ChoiceTime && canChooose == true){
			if(Input.GetKey(KeyCode.LeftArrow)){
				playingState = PlayingState.ChoiceMoving;
				recoverChoice = RecoverChoice.Left;
				anim.SetTrigger("chooseLeft");
				animTimer.SetTrigger("OnLeft");
				SelectionJudge();
			}else if(Input.GetKey(KeyCode.RightArrow)){
				playingState = PlayingState.ChoiceMoving;
				recoverChoice = RecoverChoice.Right;
				anim.SetTrigger("chooseRight");
				animTimer.SetTrigger("OnRight");
				SelectionJudge();
			}
		}
		if(score <= 0) {
			score = 0;
		}

		scoreTxt.text = score.ToString();
		timeTxt.text = (timePerRound-(int)curTime).ToString();
		if(gamingState == GamingState.GameLoding){
			if(Input.GetKey(KeyCode.Space)){
				GameStart();
				bgm.Play();
			}
		}else if(gamingState == GamingState.ResultScene){
			if(Input.GetKey(KeyCode.RightArrow)){
				ResultScene();
			}
		}


		if(curSprite.sprite == bonusLevelSprite[0]){
			if(Input.GetKey(KeyCode.LeftArrow)){
				curSprite.sprite = bonusLevelSprite[1];
				Cannnotchoose();
			}
		}else if(curSprite.sprite == bonusLevelSprite[1]){
			if(Input.GetKey(KeyCode.RightArrow)){
				curSprite.sprite = bonusLevelSprite[2];
			}
		}else if(curSprite.sprite == bonusLevelSprite[2]){
			if(Input.GetKey(KeyCode.LeftArrow)){
				curSprite.sprite = bonusLevelSprite[3];
			}
		}else if(curSprite.sprite == bonusLevelSprite[3]){
			if(Input.GetKey(KeyCode.RightArrow)){
				curSprite.sprite = bonusLevelSprite[4];
			}
		}else if(curSprite.sprite == bonusLevelSprite[4]){
			if(Input.GetKey(KeyCode.LeftArrow)){
				curSprite.sprite = bonusLevelSprite[5];
			}
		}else if(curSprite.sprite == bonusLevelSprite[5]){
			if(Input.GetKey(KeyCode.RightArrow)){
				curSprite.sprite = bonusLevelSprite[6];
			}
		}else if(curSprite.sprite == bonusLevelSprite[6]){
			if(Input.GetKey(KeyCode.LeftArrow)){
				curSprite.sprite = bonusLevelSprite[7];
			}
		}else if(curSprite.sprite == bonusLevelSprite[7]){
			if(Input.GetKey(KeyCode.RightArrow)){
				curSprite.sprite = bonusLevelSprite[8];
			}
		}else if(curSprite.sprite == bonusLevelSprite[8]){
			if(Input.GetKey(KeyCode.LeftArrow)){
				curSprite.sprite = null;
				anim.SetTrigger("bonusOff");
				score += bonusPoint;
				playingState = PlayingState.ChoiceTime;
			}
		}

		if(playingState == PlayingState.BonusLevel){
			animBonus.SetBool("isInBonusLevel",true);
		}else{
			animBonus.SetBool("isInBonusLevel",false);
		}


		if(score <= 200){
			playingSpeed = 1;
		}else if(score > 200 && score <= 500){
			playingSpeed = 2;
		}else if(score > 500 && score <= 750){
			playingSpeed = 3;
		}else if(score > 750 && score < 1000){
			playingSpeed = 4;
		}else if(score >= 1000){
			playingSpeed = 5;
		}

	}



	#region Gaming Function
	public void InitializationScene () {
		score = 0;
		curTime = 0;
		canChooose = true;
		bgm.Stop();
		crrectSound.Stop();
		wrongSound.Stop();
		timeUpSound.Stop();
	}

	public void GameStart () {
		gamingState = GamingState.Playing;
		GamingAnimControl();
	}

	private void StartCalculateTime () {
		curTime += Time.deltaTime;
		if(curTime >= timePerRound){
			TimeUp();
		}
	}

	private void TimeUp () {
		gamingState = GamingState.CalculateResult;
		isTimer = false;
		timeUpSound.Play();
		GamingAnimControl();
	}

	public IEnumerator CalculateResult () {
		yield return new WaitForSeconds(3);
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
			curTime = 0;
			isPlayingGame = true;
			break;
		case GamingState.CalculateResult:
			StartCoroutine(CalculateResult());
			break;
		case GamingState.ResultScene:
			break;
		}
	}
	#endregion


	#region Playing Fumction
	public void ReachLocation () {
		if(curMedicine.name == "medicine5"){
			Debug.Log("in");
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

	private void SelectionJudge() {
		if(recoverChoice == RecoverChoice.Right){
			if(curMedicine.number == 1 || curMedicine.number == 2 || curMedicine.number == 4 || curMedicine.number == 5){
				choiceResult = Result.Correct;
			}else if(curMedicine.number == 3){
				choiceResult = Result.Wrong;
			}
		}else if(recoverChoice == RecoverChoice.Left){
			if(curMedicine.number == 1 || curMedicine.number == 2 || curMedicine.number == 4 || curMedicine.number == 5){
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

	public void Canchoose (){
		canChooose = true;
	}

	public void Cannnotchoose (){
		canChooose = false;
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
			anim.SetTrigger("bonusOn");
			bonusLevelObj.SetActive(true);
			curSprite.sprite = bonusLevelSprite[0];
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
					wrongSound.Play();
					score -= missPoint; 
					break;
				case Result.Wrong:
					wrongSound.Play();
					score -= wrongPoint;
					break;
				case Result.Correct:
					crrectSound.Play();
					score += crrectPoint;
					break;
				}
				choiceResult = Result.Null;
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

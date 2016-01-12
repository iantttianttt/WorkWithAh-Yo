using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneVideoRander : MonoBehaviour {


	public GameManager gm; 
	public GameObject plan;
	public MovieTexture loadingGame;
	public MovieTexture loadingResult;
	public MovieTexture[] resultVideo = new MovieTexture[4];
	public Text score;
	public GameObject scoreObj;

	private bool isShow = false;
	private bool canShowResult = true;

	void Start () {
		gm = GameObject.Find("medicineRander").GetComponent<GameManager>();

	}
	

	void Update () {
		if(gm.gamingState != GamingState.ResultScene){
			scoreObj.SetActive(false);
		}
		if(gm.gamingState == GamingState.GameLoding){
			if(loadingGame.isPlaying != true){
				resultVideo[0].Stop();
				resultVideo[1].Stop();
				resultVideo[2].Stop();
				resultVideo[3].Stop();
				isShow = false;
				PlayLoadingGame();
			}
		}else if(gm.gamingState == GamingState.CalculateResult){
			if(loadingResult.isPlaying != true){
				loadingGame.Stop();
				PlayLoadingResult();
			}
		}else if(gm.gamingState == GamingState.ResultScene && canShowResult == true){
			if(gm.score <= 300){
				loadingResult.Stop();
				PlayResultVideo(0);
			}else if( 300 < gm.score && gm.score <= 600){
				loadingResult.Stop();
				PlayResultVideo(1);
			}else if( 600 < gm.score && gm.score<= 999){
				loadingResult.Stop();
				PlayResultVideo(2);
			}else if( 999 < gm.score ){
				loadingResult.Stop();
				PlayResultVideo(3);
			}
		}

		if(gm.gamingState == GamingState.Playing){
			plan.SetActive(false);
		}else{
			plan.SetActive(true);
		}
	}


	public void PlayLoadingGame () {
		plan.GetComponent<Renderer>().material.mainTexture = loadingGame;
		loadingGame.loop = true;
		loadingGame.Play();
	}

	public void PlayLoadingResult () {
		plan.GetComponent<Renderer>().material.mainTexture = loadingResult;
		loadingResult.loop = true;
		loadingResult.Play();
	}

	public void PlayResultVideo (int level) {
		if(level > resultVideo.Length){
			return;
		}
		switch(level){
		case 0:
			plan.GetComponent<Renderer>().material.mainTexture = resultVideo[0];
			resultVideo[0].Play();
			if(isShow == false){
				StartCoroutine(ShowScore());	
			}
			break;
		case 1:
			plan.GetComponent<Renderer>().material.mainTexture = resultVideo[1];
			resultVideo[1].Play();
			if(isShow == false){
				StartCoroutine(ShowScore());	
			}
			break;
		case 2:
			plan.GetComponent<Renderer>().material.mainTexture = resultVideo[2];
			resultVideo[2].Play();
			if(isShow == false){
				StartCoroutine(ShowScore());	
			}
			break;
		case 3:
			plan.GetComponent<Renderer>().material.mainTexture = resultVideo[3];
			resultVideo[3].Play();
			if(isShow == false){
				StartCoroutine(ShowScore());	
			}
			break;
		}
	}

	public IEnumerator ShowScore () {
		isShow = true;
		yield return new WaitForSeconds(3);
		scoreObj.SetActive(true);
		score.text = gm.score.ToString();
		yield return new WaitForSeconds(4);
		scoreObj.SetActive(false);
	}



}

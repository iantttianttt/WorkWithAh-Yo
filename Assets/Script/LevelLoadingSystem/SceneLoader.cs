using UnityEngine;
using System.Collections;


public class SceneLoader : MonoBehaviour {

	public enum Scenes{
		IdleAnimation = 0,
		LeadingAnimation,
		MedicineIntroduction,
		MiniGame,
		FinalReminder
	}

	public static SceneLoader ins;

	public Scenes curSceneName;

	void Awake(){
		if(ins == null){

			ins = this;
			GameObject.DontDestroyOnLoad(gameObject);
			GameSceneManager.ins.OwnSecne = (int)this.curSceneName;

		}else if(ins != this){

			Destroy(gameObject);
		}
	}

	public void LoadLevel(Scenes target){
		GameSceneManager.ins.LoadScene((int)target);
	}
}

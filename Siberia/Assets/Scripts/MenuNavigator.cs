using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour {

	public void LoadGame(){
		SceneManager.LoadScene("Game");
	}

	public void LoadGameOver(){
		SceneManager.LoadScene("Game Over");
	}
}

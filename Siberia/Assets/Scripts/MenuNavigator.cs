using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    private static string next_level;
	private static int level_no = 1;

    public void LoadGame()
    {
        MenuNavigator.level_no = 1;
		MenuNavigator.next_level = "Level " + 1;
       	LoadNextLevel();
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("Game Over");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(next_level);
    }
    
    public void LoadTutorial(){
        SceneManager.LoadScene("How To Play");
    }
    public void NextLevel()
    {
		++MenuNavigator.level_no;
        MenuNavigator.next_level = "Level " + MenuNavigator.level_no;
        if (MenuNavigator.level_no == 6)
        {
            SceneManager.LoadScene("You Won");
        }
        else
        {
            SceneManager.LoadScene("Next Level");
        }
    }
}

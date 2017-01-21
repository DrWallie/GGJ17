using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject[] pages; //0 = Main Page, 1 = Options Page, 2 = Credits Page

    public void ButtonInput(int i)
    {
        switch (i)
        {
            case 0:
                //Open name selection, Play button
                pages[0].SetActive(false);
                pages[3].SetActive(true);
                break;
            case 1:
                //Options Menu
                pages[0].SetActive(false);
                pages[1].SetActive(true);
                break;
            case 2:
                //Credits Menu
                pages[0].SetActive(false);
                pages[2].SetActive(true);
                break;
            case 3:
                //Exit Game

                break;
            case 4:
                //Close Options/Credits Pages
                pages[1].SetActive(false);
                pages[2].SetActive(false);
                pages[3].SetActive(false);
                pages[0].SetActive(true);
                break;
            case 5:
                //Join the game
                SceneManager.LoadScene(1);
                break;
        }
    }
}
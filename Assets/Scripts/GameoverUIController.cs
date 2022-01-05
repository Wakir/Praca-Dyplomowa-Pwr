using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameoverUIController : MonoBehaviour
{
    public TextMeshProUGUI resultPlaceText;
    public TextMeshProUGUI timeText;

    private RaceManager raceManager;

    private void Awake() {
        raceManager = GetComponentInParent<RaceManager>();
    }

    private void OnEnable() {
        if (GameManager.MainManager != null && GameManager.MainManager.GameState == GameState.Gameover)
        {
            string place = raceManager.GetAgentPlace(raceManager.player);
            resultPlaceText.text = place;

            float totalTime = raceManager.GetRaceTime(raceManager.player);
            int minutes = (int)totalTime / 60;
            float seconds = totalTime % 60;
            timeText.text = minutes.ToString() + ":" + seconds.ToString("0.0");
        }
    }

    public void MainMenuButton(){
        GameManager.MainManager.LoadLevel("MainMenu", GameState.MainMenu);
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI lapTimeText;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI speedText;
    public RacingCarAgent playerAgent;

    public RaceManager raceManager;
    
    private void Awake() {
        raceManager = GetComponentInParent<RaceManager>();
    }

    private void Update() {
        if(playerAgent != null)
        {
            UpdatePositionText();
            UpdateLapTimeText();
            UpdateTotalTimeText();
            UpdateLapText();
            UpdateSpeedText();
        }
    }

    private void UpdatePositionText(){
        string position = raceManager.GetAgentPlace(playerAgent);
        positionText.text = position;
    }
    private void UpdateLapTimeText(){
        float lapTime = raceManager.GetLapTime(playerAgent);
        int minutes = (int)lapTime / 60;
        float seconds = lapTime % 60;
        lapTimeText.text = "Lap time:" + minutes.ToString() + ":" + seconds.ToString("0.0");
    }
    private void UpdateTotalTimeText(){
        float totalTime = raceManager.GetRaceTime(playerAgent);
        int minutes = (int)totalTime / 60;
        float seconds = totalTime % 60;
        totalTimeText.text = "Total time: " + minutes.ToString() + ":" + seconds.ToString("0.0");
    }
    private void UpdateLapText(){
        int lap = raceManager.GetAgentLap(playerAgent);
        lapText.text = "Lap " + lap + "/" + raceManager.numLaps;
    }
    private void UpdateSpeedText(){
        int speed = raceManager.GetAgentSpeed(playerAgent);
        speedText.text = speed.ToString()+" km/h";
    }
}

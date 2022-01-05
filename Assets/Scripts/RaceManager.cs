using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
public class RaceManager : MonoBehaviour
{
    public int numLaps = 3;
    public CountDownUIController countdownUI;
    public HUDController hud;
    public GameoverUIController gameoverUI;
    public RacingArea ra;

    public RacingCarPlayerAgent player;
    private List<RacingCarAgent> racingCarAgents;

    private float lastPlaceUpdate = 0f;
    private Dictionary<RacingCarAgent, RacingCarStatus> racingStatuses;

    public float raceTimeCounter = 0;

    private class RacingCarStatus
    {
        public int checkpointIndex = 0;
        public int lap = 0;
        public int place = 0;
        public float lapTime = 0;

        public float raceTime = 0;

        public float speed = 0;
    }
    
    private void Start()
    {
        GameManager.MainManager.OnStateChange += OnStateChange;

        foreach (RacingCarAgent agent in ra.racingCarAgents)
        {
            agent.Freeze();
        }

        hud.playerAgent = player;

        hud.gameObject.SetActive(false);
        countdownUI.gameObject.SetActive(false);
        gameoverUI.gameObject.SetActive(false);

        StartCoroutine(StartRace());
    }

    private IEnumerator StartRace()
    {
        countdownUI.gameObject.SetActive(true);
        yield return countdownUI.StartCountDown();

        racingStatuses = new Dictionary<RacingCarAgent, RacingCarStatus>();
        foreach (RacingCarAgent agent in ra.racingCarAgents)
        {
            RacingCarStatus status = new RacingCarStatus();
            status.lap = 1;
            status.lapTime = 0;
            racingStatuses.Add(agent, status);
        }

        foreach (RacingCarAgent agent in ra.racingCarAgents) agent.Unfreeze();

        GameManager.MainManager.GameState = GameState.Playing;
    }
    private void OnStateChange()
    {
        if (GameManager.MainManager.GameState == GameState.Playing)
        {
            hud.gameObject.SetActive(true);
            foreach (RacingCarAgent agent in ra.racingCarAgents) agent.Unfreeze();
        }
        else if (GameManager.MainManager.GameState == GameState.Gameover)
        {
            hud.gameObject.SetActive(false);
            foreach (RacingCarAgent agent in ra.racingCarAgents) agent.Freeze();

            gameoverUI.gameObject.SetActive(true);
        }
    }

    private void FixedUpdate() {
        if (GameManager.MainManager.GameState == GameState.Playing)
        {
            if (lastPlaceUpdate + .5f < Time.fixedTime)
            {
                lastPlaceUpdate = Time.fixedTime;

                if (racingCarAgents == null)
                {
                    racingCarAgents = new List<RacingCarAgent>(ra.racingCarAgents);
                }
                racingCarAgents.Sort((a, b) => PlaceComparator(a, b));
                for (int i = 0; i < racingCarAgents.Count; i++)
                {
                    racingStatuses[racingCarAgents[i]].place = i + 1;
                }
            }

            foreach (RacingCarAgent agent in ra.racingCarAgents)
            {
                RacingCarStatus status = racingStatuses[agent];
                status.speed = 5 * agent.speed;
                if (status.checkpointIndex != agent.NextCheckpointIndex){
                    status.checkpointIndex = agent.NextCheckpointIndex;

                    if (status.checkpointIndex == 0)
                    {
                        status.lap++;
                        status.lapTime = 0;
                    }
                    else if (agent == player && status.lap > numLaps && status.checkpointIndex == 1)
                    {
                         GameManager.MainManager.GameState = GameState.Gameover;
                    }
                }
                status.lapTime += Time.fixedDeltaTime;
                status.raceTime += Time.fixedDeltaTime;
            }
        }
    }
    private int PlaceComparator(RacingCarAgent a, RacingCarAgent b)
    {
        RacingCarStatus statusA = racingStatuses[a];
        RacingCarStatus statusB = racingStatuses[b];
        // Oblicz liczbe pokonanych checkpointow
        int checkpointA = statusA.checkpointIndex + (statusA.lap - 1) * ra.Checkpoints.Count;
        int checkpointB = statusB.checkpointIndex + (statusB.lap - 1) * ra.Checkpoints.Count;
        // Porownaj wartosci checkpointow
        if (checkpointA > checkpointB){
            return -1;
        }
        else if (checkpointA < checkpointB){
            return 1;
        }
        else{
            // Oblicz odleglosc do kolejnego chechpointu.
            Vector3 nextCheckpointPosition = GetAgentNextCheckpoint(a);
            int compare = Vector3.Distance(a.transform.position, nextCheckpointPosition).CompareTo(Vector3.Distance(b.transform.position, nextCheckpointPosition));
            return compare;
        }
    }
    public Vector3 GetAgentNextCheckpoint(RacingCarAgent a)
    {
        return ra.Checkpoints[racingStatuses[a].checkpointIndex].transform.position;
    }

    private void OnDestroy() {
        if (GameManager.MainManager != null) GameManager.MainManager.OnStateChange -= OnStateChange;
    }

    public int GetAgentLap(RacingCarAgent agent)
    {
        return racingStatuses[agent].lap;
    }

    public int GetAgentSpeed(RacingCarAgent agent)
    {
        return (int)racingStatuses[agent].speed;
    }

    public string GetAgentPlace(RacingCarAgent agent)
    {
        int place = racingStatuses[agent].place;
        if(place <= 0)
        {
            return string.Empty;
        }

        switch (place)
        {
            case 1:
                return "1 st";
            case 2:
                return "2 nd";
            case 3:
                return "3 rd";
            default:
                return place.ToString() + " th";
            
        }
    }

    public float GetLapTime(RacingCarAgent agent)
    {
        return (float)Math.Round((double)racingStatuses[agent].lapTime,2);
    }

    public float GetRaceTime(RacingCarAgent agent)
    {
        return (float)Math.Round((double)racingStatuses[agent].raceTime,2);
    }
}

using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RacingArea : MonoBehaviour
{
    public CinemachineSmoothPath racePath;
    public GameObject checkpointPrefab;

    public GameObject finishLinePrefab;
    public bool trainingMode;

    public List<GameObject> Checkpoints {get; private set;}

    public List<RacingCarAgent> racingCarAgents;

    private void Awake() 
    {
        racingCarAgents = transform.GetComponentsInChildren<RacingCarAgent>().ToList();
        //Debug.Assert(RacingCarAgents.Count > 0, "No RacingAgents found");
    }

    private void Start() 
    {
        Debug.Assert(racePath != null, "Race Path was not set");
        Checkpoints = new List<GameObject>();
        int numCheckpoints = (int)racePath.MaxUnit(CinemachinePathBase.PositionUnits.PathUnits);
        for (int i = 0; i < numCheckpoints; i++)
        {
            GameObject checkpoint;
            if(i == 0) checkpoint = Instantiate<GameObject>(finishLinePrefab);
            else checkpoint = Instantiate<GameObject>(checkpointPrefab);

            checkpoint.transform.SetParent(racePath.transform);
            checkpoint.transform.localPosition = racePath.m_Waypoints[i].position;
            checkpoint.transform.rotation = racePath.EvaluateOrientationAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);

            Checkpoints.Add(checkpoint);
        }
    }
    public void ResetPosition(int chceckpoint, Rigidbody agent){
        int previousCheckpoint = chceckpoint - 1;
        if(previousCheckpoint == -1){
            previousCheckpoint = Checkpoints.Count - 1;
        }

        float checkpointPosition = racePath.FromPathNativeUnits(previousCheckpoint, CinemachinePathBase.PositionUnits.PathUnits);

        Vector3 basePosition = racePath.EvaluatePosition(checkpointPosition);

        Quaternion orientation = racePath.EvaluateOrientation(checkpointPosition);

        agent.transform.position = basePosition;
        agent.transform.rotation = orientation;
    }
}

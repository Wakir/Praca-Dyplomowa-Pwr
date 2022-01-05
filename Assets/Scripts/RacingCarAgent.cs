using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.MLAgents.Sensors;

public class RacingCarAgent : Agent
{
    //Zmienne odpowiedzialne za ruch pojazdu
    public float speed = 0; //Obecna prędkość pojazdu
    public float maxSpeed = 40; //Ograniczenie prędkości
    public float speedAcceleration = 10; //Wzrost prędkości gdy movementChange==1

    public float brakeSlowDown = 20; //Spadek prędkości gdy movementChange==2

    public float normalSlowDown = 4; //Spadek prędkości gdy movementChange==0
    public float rotationSpeed = 100; //Prędkość obrotu gdy rotationChange==1 v rotationChange==2
    private float rotationChange = 0; //Decyzja o skręcie

    private float movementChange = 0; //Decyzja o ruchu
    public int stepTimeout = 400;

    private int nextStepTimeout;

    private bool frozen = false;
    public int NextCheckpointIndex {get; set;}

    private RacingArea ra;
    public Rigidbody rb;

    private Vector3 startingLocation;

    public override void Initialize(){
        ra = GetComponentInParent<RacingArea>();
        rb = GetComponent<Rigidbody>();

        if(ra.trainingMode) MaxStep = 5000;
        else MaxStep = 0;
        startingLocation = transform.position;
    }

    public override void OnEpisodeBegin(){
        rb.velocity = Vector3.zero;
        speed = 0;
        transform.position = startingLocation;
        rb.transform.eulerAngles = Vector3.zero;
        NextCheckpointIndex = 1;

        nextStepTimeout = StepCount + stepTimeout + 90;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if(frozen) return;
        //0 - speedAcceleration, 1 - normalSlowDown, 2 -breakSlowDown
        movementChange = vectorAction[0];

        //1 - turnRight, 0 - nothing, -1 - turnLeft
        rotationChange = vectorAction[1];
        if(rotationChange == 2) rotationChange = -1;

        MovementProcess();

        if(StepCount > nextStepTimeout || stepTimeout==0){
            if(!rb.CompareTag("player")){
                EndEpisode();
            }
        }

        if (ra.trainingMode){
            AddReward(-1f / MaxStep);
        }
    }

    public override void CollectObservations(VectorSensor sensor){
        //Obserwuj wektor prędkości (3)
        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));
        //Obserwuj drogę do kolejnego checkpointu (3)
        sensor.AddObservation(VectorToNextCheckpoint());
    }
    public void Freeze()
    {
        frozen = true;
        rb.Sleep();
    }

    public void Unfreeze()
    {
        frozen = false;
        rb.WakeUp();
    }

    private Vector3 VectorToNextCheckpoint()
    {
        Vector3 nextCheckpointDir = ra.Checkpoints[NextCheckpointIndex].transform.position - transform.position;
        Vector3 localCheckpointDir = transform.InverseTransformDirection(nextCheckpointDir);
        return localCheckpointDir;
    }

    void MovementProcess(){
        moveCar();
        rotateCar();
    }

    void moveCar(){
        if(movementChange == 1){
            speed += speedAcceleration*Time.fixedDeltaTime;
        }
        else if(movementChange == 2){
            speed -= brakeSlowDown*Time.fixedDeltaTime;
        }
        else if(movementChange == 0){
            speed -= normalSlowDown*Time.fixedDeltaTime;
        }
        speed = SpeedLimitation(speed);
        //Przeniesienie obecnej wartości prędkości z lokalnej wartości
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0;
        localVelocity.y = 0;
        localVelocity.z = speed;
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    float SpeedLimitation(float speed){
        return Mathf.Max(Mathf.Min(speed, maxSpeed),0);
    }

    void rotateCar(){
        //Zmienna rotationChange uległa zmianie dla wartości 2 na -1 w funkcji onActionRecived.
        transform.Rotate(Vector3.up * rotationSpeed * rotationChange * Time.fixedDeltaTime);
    }
    private void OnCollisionEnter(Collision other) {
        speed = 0;
        if (ra.trainingMode && !other.transform.CompareTag("agent"))
        {
            AddReward(-1f);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("checkpoint") && other.gameObject == ra.Checkpoints[NextCheckpointIndex]){
            GotCheckpoint();
        }
    }

    private void GotCheckpoint()
    {
        NextCheckpointIndex = (NextCheckpointIndex + 1) % ra.Checkpoints.Count;

        if (ra.trainingMode){
            AddReward(.5f);
        }
        nextStepTimeout = StepCount + stepTimeout;
    }
}

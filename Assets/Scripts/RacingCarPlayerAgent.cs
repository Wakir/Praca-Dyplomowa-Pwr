using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.InputSystem;

public class RacingCarPlayerAgent : RacingCarAgent
{
    public InputAction movementInput;
    public InputAction rotateInput;
    public InputAction pauseInput;

    public override void Initialize()
    {
        base.Initialize();
        movementInput.Enable();
        rotateInput.Enable();
        pauseInput.Enable();
    }

    private void OnDestroy() {
        movementInput.Disable();
        rotateInput.Disable();
        pauseInput.Disable();
    }

    public override void Heuristic(float[] actionsOut)
    {
        //Move: 1 == speedAcceleration, 0 == - normalSlowDown, -1 == -breakSlowDown
        float movementValue = Mathf.Round(movementInput.ReadValue<float>());

        // Rotate: 1 == turn right, 0 - nothing, -1 == turn left
        float rotationValue = Mathf.Round(rotateInput.ReadValue<float>());

        if(movementValue == -1f) movementValue = 2f;
        if(rotationValue == -1f) rotationValue = 2f;

        actionsOut[0] = movementValue;
        actionsOut[1] = rotationValue;
    }
}

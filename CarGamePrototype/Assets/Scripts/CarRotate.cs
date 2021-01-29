using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRotate : MonoBehaviour
{
    /// <summary>
    /// Send rotation input pressed and holded
    /// </summary>
    /// <param name="isRight">Is right movement</param>
    public void RotateCarPressDown(bool isRight)
    {
        //Send input to current player car
        LevelManager.currentCar.RotationInput(isRight ? 1 : -1);

        //If it is first input in initiated turn, send first player input event
        if (LevelManager.current.IsTurnInitiated && !LevelManager.current.IsTurnStarted)
            LevelEvents.current.FirstPlayerInput();
    }

    /// <summary>
    /// Send rotation input released
    /// </summary>
    public void RotateCarPressUp()
    {
        //Send input to current player car
        LevelManager.currentCar.RotationInput(0);
    }
}

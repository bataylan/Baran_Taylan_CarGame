using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI Car Rotation Button
/// </summary>
public class CarRotateButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //Button role
    public bool IsRightMovementButton;

    /// <summary>
    /// Detect is UI button clicked
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        //Send input to current player car
        LevelManager.currentCar.RotationInput(IsRightMovementButton ? 1 : -1);

        //If it is first input in initiated turn, send first player input event
        if (LevelManager.current.IsTurnInitiated && !LevelManager.current.IsTurnStarted)
            LevelEvents.current.FirstPlayerInput();
    }

    /// <summary>
    /// Detects is UI button released
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        //Send input to current player car
        LevelManager.currentCar.RotationInput(0);
    }
}

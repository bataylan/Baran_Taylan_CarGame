using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI Car Rotation Button
/// </summary>
public class CarRotateButtonUI : CarRotate, IPointerDownHandler, IPointerUpHandler
{
    //Button role
    public bool IsRightMovementButton;

    /// <summary>
    /// Detect is UI button clicked
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        RotateCarPressDown(IsRightMovementButton);
    }

    /// <summary>
    /// Detects is UI button released
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        RotateCarPressUp();
    }
}

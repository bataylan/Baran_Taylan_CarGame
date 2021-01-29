using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurnPointInfo : MonoBehaviour
{
    //Turn objects
    public Transform EntrancePoint;
    public Transform ExitPoint;
    public Transform Car;

    //Turn info
    public int TurnIndex;

    [SerializeField]
    private TextMesh EntrancePointText;
    [SerializeField]
    private TextMesh ExitPointText;

    [SerializeField]
    private TextMesh EntrancePointIndexText;
    [SerializeField]
    private TextMesh ExitPointIndexText;

    /// <summary>
    /// Set visibility
    /// </summary>
    /// <param name="isHide"></param>
    public void HideObjects(bool isHide)
    {
        if (isHide)
        {
            //Set colors to transparent
            EntrancePointText.color = Color.clear;
            ExitPointText.color = Color.clear;

            EntrancePointIndexText.color = Color.clear;
            ExitPointIndexText.color = Color.clear;

            //Deactivate model car
            Car.gameObject.SetActive(false);
        }
        else
        {
            //Set color visible
            EntrancePointText.color = Color.white;
            ExitPointText.color = Color.white;

            EntrancePointIndexText.color = Color.yellow;
            ExitPointIndexText.color = Color.yellow;

            //Activate car
            Car.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Update texts based on index
    /// </summary>
    public void UpdateTurnPointInfo()
    {
        //Update game objects texts
        if (EntrancePointIndexText)
            EntrancePointIndexText.text = TurnIndex.ToString();

        if (ExitPointIndexText)
            ExitPointIndexText.text = TurnIndex.ToString();

        //Set parent object name based on turn index
        gameObject.name = "TurnInfoPoint-" + TurnIndex;
    }

    /// <summary>
    /// Get entrance point position
    /// </summary>
    /// <returns>Entrance Point Transform Position</returns>
    public Vector3 GetEntrancePointPosition()
    {
        if (!EntrancePoint)
        {
            Debug.LogError("Entrance point is null on TurnPointInfo " + TurnIndex);
            return Vector3.zero;
        }

        return EntrancePoint.position;
    }

    /// <summary>
    /// Get exit point position
    /// </summary>
    /// <returns>Exit Point Transform Position</returns>
    public Vector3 GetExitPointPosition()
    {
        if (!ExitPoint)
        {
            Debug.LogError("Exit point is null on TurnPointInfo " + TurnIndex);
            return Vector3.zero;
        }

        return ExitPoint.position;
    }

    /// <summary>
    /// Get car spawn position
    /// </summary>
    /// <returns>Car Spawn Transform Position</returns>
    public Vector3 GetCarSpawnPointPosition()
    {
        if (!Car)
        {
            Debug.LogError("Car spawn point is null on TurnPointInfo " + TurnIndex);
            return Vector3.zero;
        }

        return Car.position;
    }

    /// <summary>
    /// Get car spawn rotation
    /// </summary>
    /// <returns>Car Spawn Transform Rotation</returns>
    public Quaternion GetCarSpawnPointRotation()
    {
        if (!Car)
        {
            Debug.LogError("Car spawn rotation is null on TurnPointInfo " + TurnIndex);
            return Quaternion.identity;
        }

        return Car.rotation;
    }
}
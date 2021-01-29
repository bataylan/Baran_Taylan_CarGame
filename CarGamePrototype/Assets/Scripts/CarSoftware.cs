using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player and bot car software
/// </summary>
public class CarSoftware : MonoBehaviour
{
    //Car body
    public GameObject CarBody;
    public GameObject MainBody;
    public Color BotColor;

    //Car info
    private bool _isPlayerCar;
    private int _turnIndex;

    //Car movement settings
    private float _accelerationFactor = 1f;
    private float _maxSpeed = 5f;
    private float _rotationFactor = 1f;
    private float _movementPrecision;

    //Start transform datas
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    //Current status info
    public int MovementCounter;
    private float _currentSpeed = 0f;
    private bool _isCarMovementEnabled;
    private int _carRotation;

    //Rotate input info
    private bool _isThereNewRotateInput;
    private int _newRotateInput;

    //Record datas
    private List<int> _recordedRotations;
    private List<int> _recordedIntervals;
    private int _recordIntervalIndex;
    private int _recordIndex;

    private void Awake()
    {
        LevelManager.currentCar = this;
        _isPlayerCar = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _movementPrecision = LevelManager.current.MovementPrecision;

        ResetRecords();

        LevelEvents.current.OnTurnStart += StartCarMovement;

        LevelEvents.current.OnRestartActiveCars += ResetCar;
        LevelEvents.current.OnMakeActiveCarsBot += SetAsBotCar;
    }

    /// <summary>
    /// This methods used like constructor
    /// since constructor can't be used because of
    /// MonoBehaviour inheritance
    /// </summary>
    /// <param name="turnIndex"></param>
    /// <param name="maxSpeed"></param>
    /// <param name="accelerationFactor"></param>
    /// <param name="rotationFactor"></param>
    /// <param name="startPosition"></param>
    /// <param name="startRotation"></param>
    public void SetCarProperties(int turnIndex, float maxSpeed, float accelerationFactor,
        float rotationFactor, Vector3 startPosition, Quaternion startRotation)
    {
        _turnIndex = turnIndex;
        _maxSpeed = maxSpeed;
        _accelerationFactor = accelerationFactor;
        _rotationFactor = rotationFactor;

        _startPosition = startPosition;
        _startRotation = startRotation;
    }

    #region Rotation
    /// <summary>
    /// Set rotation fields with new value
    /// </summary>
    /// <param name="rotation">Left(-1) / Forward(0) / Right(1) rotation</param>
    public void RotationInput(int rotation)
    {
        //Check that rotation input is different then current rotation
        if (rotation == _carRotation)
            return;

        //set new rotation input for timed methods use
        _newRotateInput = rotation;
        _isThereNewRotateInput = true;
    }

    /// <summary>
    /// Changes rotation
    /// </summary>
    private void ChangeRotation()
    {
        //Change rotation if there is new input
        if (_isThereNewRotateInput)
        {
            _carRotation = _newRotateInput;

            //Record new rotation if it is player car
            if (_isPlayerCar)
                RecordRotation();

            _isThereNewRotateInput = false;
        }
    }
    #endregion

    #region Record
    /// <summary>
    /// Reset record related data
    /// </summary>
    private void ResetRecords()
    {
        _recordedRotations = new List<int>();
        _recordedIntervals = new List<int>();
        _recordedRotations.Add(0);
        _recordedIntervals.Add(0);
        _recordIntervalIndex = 0;
        _recordIndex = 0;
    }

    /// <summary>
    /// Record rotation input with movement interval
    /// </summary>
    private void RecordRotation()
    {
        //Hold rotation input with interval
        //current index's rotation is current index + 1's movement interval
        _recordedRotations.Add(_carRotation);
        _recordedIntervals.Add(_recordIntervalIndex);
        _recordIndex++;
        _recordIntervalIndex = 0;
    }

    /// <summary>
    /// Count intervals for rotation recording
    /// </summary>
    private void RecordIndexCounter()
    {
        //count movement interval
        _recordIntervalIndex++;
    }
    #endregion

    #region Replay
    /// <summary>
    /// Check recorded replay input based on recorded intervals
    /// </summary>
    private void CheckIsThereReplayInput()
    {
        if (_recordIntervalIndex <= 0)
            ReplayRotation();
    }

    /// <summary>
    /// Replay recorded rotation input
    /// </summary>
    private void ReplayRotation()
    {
        if (_recordIndex + 1 >= _recordedRotations.Count)
            return;

        RotationInput(_recordedRotations[_recordIndex]);
        _recordIntervalIndex = _recordedIntervals[_recordIndex + 1];
        _recordIndex++;
    }

    /// <summary>
    /// Countdown recorded rotation intervals
    /// </summary>
    private void ReplayIndexCountdown()
    {
        _recordIntervalIndex--;
    }
    #endregion

    #region Movement
    /// <summary>
    /// Initiate car movement
    /// </summary>
    public void StartCarMovement()
    {
        //start car movement
        _isCarMovementEnabled = true;
        StartCoroutine(MoveWithSpeed());
    }

    /// <summary>
    /// Stop car movement
    /// </summary>
    public void StopCarMovement()
    {
        _isCarMovementEnabled = false;

        //if it is player car, record last movement
        if (_isPlayerCar)
            RecordRotation();
        StopAllCoroutines();
    }

    /// <summary>
    /// Move, rotate and accelerate car with 
    /// constant rates based on rotation input
    /// </summary>
    IEnumerator MoveWithSpeed()
    {
        while (_isCarMovementEnabled)
        {
            //count every movement for info and debugging
            MovementCounter++;

            //movement count for rotation record/replay
            if (_isPlayerCar)
                RecordIndexCounter();
            else
                ReplayIndexCountdown();

            //if it is a bot car, check is there replay rotation for movement
            if (!_isPlayerCar)
                CheckIsThereReplayInput();

            //change rotation based on input
            ChangeRotation();

            //calculate speed with acceleration on every movement
            Accelerate();
            gameObject.transform.Translate(gameObject.transform.forward * _currentSpeed * _movementPrecision, Space.World);

            //rotate car left/right
            if (_carRotation != 0)
            {
                var rotation = Quaternion.LookRotation(_carRotation * transform.right);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _movementPrecision * _rotationFactor);
            }

            yield return new WaitForSeconds(_movementPrecision);
        }
    }

    /// <summary>
    /// Set current speed with lerp for acceleration
    /// </summary>
    void Accelerate()
    {
        //calculate speed with acceleration
        _currentSpeed = Mathf.Lerp(_currentSpeed, _maxSpeed, _movementPrecision * _accelerationFactor);
    }
    #endregion

    #region Turn Based
    /// <summary>
    /// Reset car for new turns
    /// </summary>
    public void ResetCar()
    {
        //stop car movement
        StopAllCoroutines();

        //hide car body
        HideCar();

        //move car to initial position and rotation
        gameObject.transform.position = _startPosition;
        gameObject.transform.rotation = _startRotation;

        _currentSpeed = 0;
        MovementCounter = 0;
        _carRotation = 0;

        //reset rotation input
        _isThereNewRotateInput = false;
        _newRotateInput = 0;

        //reset record/replay counter and index
        _recordIntervalIndex = 0;
        _recordIndex = 0;
        
        //replay rotation as players first input
        ReplayRotation();

        //show car body
        ShowCar();
    }

    /// <summary>
    /// Hide car body on game
    /// </summary>
    private void HideCar()
    {
        CarBody.SetActive(false);
    }

    /// <summary>
    /// Show car body on game
    /// </summary>
    private void ShowCar()
    {
        CarBody.SetActive(true);
    }

    /// <summary>
    /// Change car from player car to bot car
    /// </summary>
    private void SetAsBotCar()
    {
        _isPlayerCar = false;
        ChangeMainBodyColor();
    }

    /// <summary>
    /// Change body color of car
    /// </summary>
    private void ChangeMainBodyColor()
    {
        if (!MainBody)
            return;

        var renderer = MainBody.gameObject.GetComponent<Renderer>();
        if (!renderer)
            return;

        renderer.material.SetColor("_Color", BotColor);
    }
    #endregion

    #region Point and car collision
    /// Detects collisions with collider
    private void OnTriggerEnter(Collider col)
    {
        //if it is a player car send events based on collided object
        //for turn controls
        if (_isPlayerCar)
        {
            if (col.gameObject.CompareTag("Car"))
            {
                //arrange car for restart
                StopCarMovement();
                ResetRecords();
                LevelEvents.current.PlayerCarCrashWithAnotherCar();
            }
            else if (col.gameObject.CompareTag("Obstecle"))
            {
                //arrange car for restart
                StopCarMovement();
                ResetRecords();
                LevelEvents.current.PlayerCarObstecleCrash();
            }
            else if (CheckThatIsCurrentCarExitPoint(col.gameObject))
            {
                //if it is arrive point don't reset records for replay purposes
                //only stop movement
                StopCarMovement();
                LevelEvents.current.PlayerCarArriveExit();
            }
        }
        else if (col.gameObject.CompareTag("Car") || col.gameObject.CompareTag("Obstecle")
            || CheckThatIsCurrentCarExitPoint(col.gameObject))
        {
            //if it is a bot car only stop movement
            StopCarMovement();
        }
    }

    /// <summary>
    /// Check that game object is same as car's exit point
    /// </summary>
    /// <param name="toCheckObject">Exit point</param>
    /// <returns>Is car's exit point</returns>
    private bool CheckThatIsCurrentCarExitPoint(GameObject toCheckObject)
    {
        //check is object exit point by game object tag
        if (!toCheckObject || !toCheckObject.CompareTag("ExitPoint"))
            return false;

        var exitPoint = toCheckObject.GetComponent<ExitPoint>();
        if (!exitPoint)
            return false;

        //check is exit point belong to car by turn index
        return exitPoint.TurnIndex == _turnIndex;
    }
    #endregion
}

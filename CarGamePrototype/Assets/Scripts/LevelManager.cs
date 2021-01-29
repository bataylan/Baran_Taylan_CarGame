using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Level logic manager
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager current;
    public static CarSoftware currentCar;

    //Car settings
    public float MovementPrecision;
    public float CarMaxSpeed;
    public float CarAccelerationFactor;
    public float CarRotationFactor;

    //Car game objects
    public GameObject CarPrefab;
    public GameObject CarsParent;

    //Turn point game objects
    public GameObject TurnPointInfosParent;
    public GameObject TurnPointsParent;
    public GameObject EntrancePrefab;
    public GameObject ExitPrefab;

    //Turn status
    [HideInInspector]
    public bool IsTurnInitiated;
    [HideInInspector]
    public bool IsTurnStarted;

    //Level end UI game objects
    public GameObject LevelEndUI;
    public Button NextLevelButton;
    public Text PlayedTurnCountText;

    //Current turn counter
    public int CurrentTurnIndex;

    private List<TurnPointInfo> _turnsPointInfos;
    private int _playedTurnsCount;

    private void Awake()
    {
        current = this;
        PrepareTurnPointInfoList();
        HideAllTurnPoints();
    }

    // Start is called before the first frame update
    void Start()
    {
        LevelEvents.current.OnPlayerCarArriveExit += CompleteTurn;
        LevelEvents.current.OnFirstPlayerInput += StartTurn;

        //On accident restart turn
        LevelEvents.current.OnPlayerCarCrashWithAnotherCar += RestartTurn;
        LevelEvents.current.OnPlayerCarObstecleCrash += RestartTurn;

        StartLevel();
    }

    #region Utilities
    /// <summary>
    /// Get car spawn rotation from turn point info
    /// </summary>
    /// <returns>Car spawn rotation</returns>
    private Quaternion GetCarSpawnRotation()
    {
        if (_turnsPointInfos[CurrentTurnIndex] == null || _turnsPointInfos[CurrentTurnIndex].Car == null)
            return Quaternion.identity;

        return _turnsPointInfos[CurrentTurnIndex].GetCarSpawnPointRotation();
    }

    /// <summary>
    /// Get car spawn position from turn point info
    /// </summary>
    /// <returns>Car spawn position</returns>
    private Vector3 GetCarSpawnPosition()
    {
        if (_turnsPointInfos[CurrentTurnIndex] == null || _turnsPointInfos[CurrentTurnIndex].Car == null)
            return Vector3.zero;

        return _turnsPointInfos[CurrentTurnIndex].GetCarSpawnPointPosition();
    }

    /// <summary>
    /// Hide all turn points from in game
    /// </summary>
    private void HideAllTurnPoints()
    {
        if (_turnsPointInfos == null || _turnsPointInfos.Count <= 0)
            return;

        for (int i = 0; i < _turnsPointInfos.Count; i++)
        {
            if (_turnsPointInfos[i])
                _turnsPointInfos[i].HideObjects(true);
        }
    }
    #endregion

    #region Arrange Turns
    /// <summary>
    /// Set turn as started
    /// </summary>
    public void StartTurn()
    {
        //Start turn only if it is initiated but not started
        if (IsTurnInitiated && !IsTurnStarted)
        {
            IsTurnStarted = true;
            Time.timeScale = 1f;

            //Send turn started event
            LevelEvents.current.TurnStart();
        }
    }

    /// <summary>
    /// Set turn as stopped
    /// </summary>
    public void StopTurn()
    {
        //Stop turn if it is started
        if (IsTurnStarted)
        {
            IsTurnStarted = false;
            Time.timeScale = 0f;

            //Send turn stopped event
            LevelEvents.current.TurnStop();
        }
    }

    /// <summary>
    /// Restarts the turn
    /// </summary>
    public void RestartTurn()
    {
        //Stop turn first
        StopTurn();

        //Restart all cars in scene (player and bots)
        LevelEvents.current.RestartActiveCars();

        //Initiate turn as restart, not a new turn
        InitiateTurn(isRestart: true);
    }

    /// <summary>
    /// Complete and ends/start next turn
    /// </summary>
    public void CompleteTurn()
    {
        //Send turn completed event
        LevelEvents.current.TurnCompleted();

        //Check is there next turns
        if (IsLevelCompleted())
        {
            EndLevel();
        }
        else
        {
            PrepareNextTurn();
        }
    }

    /// <summary>
    /// Prepare next turns for start
    /// </summary>
    private void PrepareNextTurn()
    {
        //Stop turn first
        StopTurn();

        //Restart all cars
        LevelEvents.current.RestartActiveCars();

        //Set active cars as bot
        LevelEvents.current.MakeActiveCarsBot();

        CurrentTurnIndex++;

        InitiateTurn();
    }

    /// <summary>
    /// Initiate turn with spawning necessery objects
    /// </summary>
    public void InitiateTurn(bool isRestart = false)
    {
        //Check current index is lower than level limit
        if (CurrentTurnIndex >= _turnsPointInfos.Count)
            return;

        //Freeze time
        Time.timeScale = 0f;

        //Execute if it is a new turn only
        if (!isRestart)
        {
            //Destroy existing turn points
            ArrangePastTurnPoints();

            //Initiate turn points
            InitiateEntrance();
            InitiateExit();

            //Initiate player car
            SpawnCar();
        }

        //Set turn status as initiated
        IsTurnInitiated = true;
        //Count turn as played initially
        _playedTurnsCount++;
    }

    /// <summary>
    /// Spawn car for player
    /// </summary>
    /// <returns>Spawned car</returns>
    public GameObject SpawnCar()
    {
        //Check are necessery game objects are exist
        if (_turnsPointInfos == null || _turnsPointInfos.Count <= 0 || !CarPrefab || !CarsParent)
            return null;

        //Get car initial position and rotation
        var spawnPosition = GetCarSpawnPosition();
        var spawnRotation = GetCarSpawnRotation();

        //Create player car on game
        GameObject spawnedCar = Instantiate(parent: CarsParent.transform,
                        original: CarPrefab.gameObject, position: spawnPosition, rotation: spawnRotation) as GameObject;

        //Set car's initial info (like constructor)
        var carSoftware = spawnedCar.gameObject.GetComponent<CarSoftware>();
        if (carSoftware)
        {
            carSoftware.SetCarProperties(turnIndex: CurrentTurnIndex, maxSpeed: CarMaxSpeed,
                accelerationFactor: CarAccelerationFactor, rotationFactor: CarRotationFactor,
                startPosition: spawnPosition, startRotation: spawnRotation);
        }

        return spawnedCar;
    }
    #endregion

    #region Turn Point Methods
    /// <summary>
    /// Prepare turn points list from parent object
    /// </summary>
    private void PrepareTurnPointInfoList()
    {
        //Create new list for turn point infos
        _turnsPointInfos = new List<TurnPointInfo>();
        if (!TurnPointInfosParent)
            return;

        //Add active childrens of turn point info game object in list
        for (int i = 0; TurnPointInfosParent.transform.childCount > i; i++)
        {
            var turnPointInfoObject = TurnPointInfosParent.transform.GetChild(i);
            if (turnPointInfoObject && turnPointInfoObject.gameObject.activeSelf)
            {
                var turnPointInfoScript = turnPointInfoObject.GetComponent<TurnPointInfo>();
                if (turnPointInfoScript)
                {
                    _turnsPointInfos.Add(turnPointInfoScript);
                }
            }
        }

        //Order them based on turn index
        _turnsPointInfos = _turnsPointInfos.OrderBy(x => x.TurnIndex).ToList();
    }

    /// <summary>
    /// Create entrance point in game
    /// </summary>
    /// <returns>Spawned entrance point game object</returns>
    public GameObject InitiateEntrance()
    {
        //Check that necessery infos are exist
        if (_turnsPointInfos[CurrentTurnIndex] == null || _turnsPointInfos[CurrentTurnIndex].EntrancePoint == null
             || !TurnPointsParent)
            return null;

        //Spawn entrance point for active turn
        GameObject spawnedEntrancePoint = Instantiate(parent: TurnPointsParent.transform,
                        original: EntrancePrefab.gameObject, position: _turnsPointInfos[CurrentTurnIndex].GetEntrancePointPosition(),
                        rotation: Quaternion.identity) as GameObject;

        return spawnedEntrancePoint;
    }

    /// <summary>
    /// Create exit point in game
    /// </summary>
    /// <returns>Spawned exit point game object</returns>
    public GameObject InitiateExit()
    {
        //Check that necessery infos are exist
        if (_turnsPointInfos[CurrentTurnIndex] == null || _turnsPointInfos[CurrentTurnIndex].ExitPoint == null
            || !TurnPointsParent)
            return null;

        //Spawn entrance point for active turn
        GameObject spawnedExitPoint = Instantiate(parent: TurnPointsParent.transform,
                        original: ExitPrefab.gameObject, position: _turnsPointInfos[CurrentTurnIndex].GetExitPointPosition(),
                        rotation: Quaternion.identity) as GameObject;

        //Set exit point's
        var exitPoint = spawnedExitPoint.gameObject.GetComponent<ExitPoint>();
        if (!exitPoint)
            return null;

        exitPoint.TurnIndex = CurrentTurnIndex;

        return spawnedExitPoint;
    }

    /// <summary>
    /// Destroy entrance and hides exit points
    /// </summary>
    public void ArrangePastTurnPoints()
    {
        for (int i = 0; TurnPointsParent.transform.childCount > i; i++)
        {
            //Hide exit points texts but not destroy them for bot cars
            var turnPoint = TurnPointsParent.transform.GetChild(i);
            if (turnPoint.CompareTag("ExitPoint"))
            {
                var exitPoint = turnPoint.GetComponent<ExitPoint>();
                if (exitPoint)
                    exitPoint.HideText();
            }
            else //Destroy entrance points of bot cars
            {
                Destroy(turnPoint.gameObject);
            }
        }
    }
    #endregion

    #region Level Methods
    /// <summary>
    /// Start level with initiating first turn
    /// </summary>
    private void StartLevel()
    {
        InitiateTurn();
    }

    /// <summary>
    /// Set level status stopped and open level and panel
    /// </summary>
    private void EndLevel()
    {
        //Stop turn first
        StopTurn();

        //Set turn status as not started and initiated
        IsTurnInitiated = false;
        IsTurnStarted = false;

        //Prepare and open level end UI
        OpenLevelEndUI();
    }

    /// <summary>
    /// Open level and panel
    /// </summary>
    private void OpenLevelEndUI()
    {
        //Prepare level end UI objects
        PlayedTurnCountText.text = _playedTurnsCount.ToString();
        NextLevelButton.onClick.AddListener(delegate () { OpenNextLevel(); });

        if (IsThereNextLevel())
            NextLevelButton.interactable = true;
        else
            NextLevelButton.interactable = false;

        //Show it to player
        LevelEndUI.SetActive(true);
    }

    /// <summary>
    /// Open next level scene
    /// </summary>
    public void OpenNextLevel()
    {
        //Check that is there next level
        if (!IsThereNextLevel())
            return;

        //Load next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Check that is level completed
    /// </summary>
    /// <returns>Is level completed</returns>
    private bool IsLevelCompleted()
    {
        //Check level completion by turn point counts
        return _turnsPointInfos != null && _turnsPointInfos.Count > 0
            && CurrentTurnIndex >= _turnsPointInfos.Count - 1;
    }

    /// <summary>
    /// Check that is there next level in game
    /// </summary>
    /// <returns>Is there next level</returns>
    private bool IsThereNextLevel()
    {
        //Check that is there next level by try to find next scene
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        var nextLevelScenePath = SceneUtility.GetScenePathByBuildIndex(currentSceneIndex + 1);
        return !string.IsNullOrWhiteSpace(nextLevelScenePath);
    }
    #endregion
}
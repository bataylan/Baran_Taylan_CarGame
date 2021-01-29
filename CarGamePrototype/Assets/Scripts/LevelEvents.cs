using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Level events
/// </summary>
public class LevelEvents : MonoBehaviour
{
	public static LevelEvents current;
	
	void Awake()
	{
		current = this;
	}

    //Turn completed event
    public event Action OnTurnCompleted;
    public void TurnCompleted()
    {
        OnTurnCompleted?.Invoke();
    }

    //Player driven car obstecle crash event
    public event Action OnPlayerCarObstecleCrash;
    public void PlayerCarObstecleCrash()
    {
        OnPlayerCarObstecleCrash?.Invoke();
    }

    //Player driven car crashed with bot car event
    public event Action OnPlayerCarCrashWithAnotherCar;
    public void PlayerCarCrashWithAnotherCar()
    {
        OnPlayerCarCrashWithAnotherCar?.Invoke();
    }

    //Player driven car exit point arrive event
    public event Action OnPlayerCarArriveExit;
    public void PlayerCarArriveExit()
    {
        OnPlayerCarArriveExit?.Invoke();
    }

    //Player car created event
    public event Action OnPlayerCarInstantiated;
    public void PlayerCarInstantiated()
    {
        OnPlayerCarInstantiated?.Invoke();
    }

    //Turn started event
    public event Action OnTurnStart;
    public void TurnStart()
    {
        OnTurnStart?.Invoke();
    }

    //Turn stopped event
    public event Action OnTurnStop;
    public void TurnStop()
    {
        OnTurnStop?.Invoke();
    }

    //First player input event
    public event Action OnFirstPlayerInput;
    public void FirstPlayerInput()
    {
        OnFirstPlayerInput?.Invoke();
    }

    //Restart active cars event
    public event Action OnRestartActiveCars;
    public void RestartActiveCars()
    {
        OnRestartActiveCars?.Invoke();
    }

    //Make active cars bot event
    public event Action OnMakeActiveCarsBot;
    public void MakeActiveCarsBot()
    {
        OnMakeActiveCarsBot?.Invoke();
    }
}
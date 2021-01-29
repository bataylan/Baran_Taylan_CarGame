using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    public int TurnIndex;

    public GameObject Text;

    public void HideText()
    {
        if (!Text)
            return;

        Text.SetActive(false);
    }
}
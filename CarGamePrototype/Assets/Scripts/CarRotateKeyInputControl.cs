using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRotateKeyInputControl : CarRotate
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateCarPressDown(true);
        }
        else if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            RotateCarPressUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateCarPressDown(false);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            RotateCarPressUp();
        }
    }
}

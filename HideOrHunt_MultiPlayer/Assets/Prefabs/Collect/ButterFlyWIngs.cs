using UnityEngine;

public class ButterFlyWIngs : MonoBehaviour
{
    public GameObject wingLeft; 
    public GameObject wingRight; 
    public int maxRotationY = 70; 
    public float rotationSpeed = 80; 

    private bool increasing = true; 

    void Update()
    {
        float leftWingRotationY = wingLeft.transform.localEulerAngles.y;
        float rightWingRotationY = wingRight.transform.localEulerAngles.y;
        
        leftWingRotationY = (leftWingRotationY > 180) ? leftWingRotationY - 360 : leftWingRotationY;
        rightWingRotationY = (rightWingRotationY > 180) ? rightWingRotationY - 360 : rightWingRotationY;

        
        if (increasing)
        {
            wingLeft.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            wingRight.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        else
        {
            wingLeft.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            wingRight.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        if (leftWingRotationY >= maxRotationY || rightWingRotationY <= -maxRotationY)
        {
            increasing = false;
        }
        else if (leftWingRotationY <= 0 || rightWingRotationY >= 0)
        {
            increasing = true;
        }
    }
}
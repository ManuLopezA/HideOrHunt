using System;
using UnityEngine;
using Unity.Mathematics;

public class ButterFlyWings01 : MonoBehaviour
{
    public GameObject leftWing;  
    public GameObject rightWing; 
    public float maxRotationY = 30f;
    public float minRotationY = -60f;
    public float rotationSpeed = 60f;  

    private float currentRotationY = 0f;  // Rotación actual en el eje Y
    private bool increasing = true;  // Indica si la rotación está aumentando o disminuyendo

 

    void Update()
    {
        // Actualizar la rotación en el eje Y
        
        if (increasing)
        {
            currentRotationY += rotationSpeed * Time.deltaTime;
            if (currentRotationY >= maxRotationY)
            {
                currentRotationY = maxRotationY;
                increasing = false;
            }
        }
        else
        {
            currentRotationY -= rotationSpeed * Time.deltaTime;
            if (currentRotationY <= 0)
            {
                currentRotationY = 0;
                increasing = true;
            }
        }

        leftWing.transform.rotation = Quaternion.Euler(0f, currentRotationY, 0);
        rightWing.transform.rotation = Quaternion.Euler(0f, maxRotationY - currentRotationY, 0);}
}

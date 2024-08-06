using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAndDownMovement : MonoBehaviour
{
    [SerializeField] private float maxHeight = 2.5f;
    [SerializeField] private float minHeight = 2f; 
    [SerializeField] private float movementSpeed = 0.3f; 

    private bool movingUp = true;

    void Update()
    {
        float direction = (movingUp) ? 1f : -1f;
        float movementDelta = movementSpeed * Time.deltaTime * direction;

        transform.Translate(0f, movementDelta, 0f);

       if ((movingUp && transform.position.y >= maxHeight) || (!movingUp && transform.position.y <= minHeight))
        {
            movingUp = !movingUp;
        }
    }
}

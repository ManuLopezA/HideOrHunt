using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MapMiniature : MonoBehaviour
{
    private bool isMoving;
    [SerializeField] private int index;
    [SerializeField] private float speed = 20f;
    [SerializeField] private SelectionMapManager selectionMapManager;
    [SerializeField] private GameObject miniature;
    private Transform[] points;
    
    private MeshRenderer mesh;
    private SpriteRenderer sprite;
    private TextMeshPro text;
    
    private Color white = Color.white;
    private Color blueLight = new Color(0.6f, 0.8f, 1.0f); 
    private Color blueMedium = new Color(0.0f, 0.0f, 0.4f);
    private Color blueDark = new Color(0.0f, 0.0f, 0.2f); 
    
    private void Start()
    {
        points = selectionMapManager.Points();
        mesh = GetComponentInChildren<MeshRenderer>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
        UpdateColor();
    }
    
    private void Update()
    {
        if (isMoving)
        {
            UpdateColor();
        }
    }
    
    public void GoNextPosition()
    {
        if (isMoving) return;
        index++;
        if (index == points.Length) index = 0;
        StartCoroutine(MoveToPosition(points[index].position));
    }

    public void GoPrevPosition()
    {
        if (isMoving) return;
        index--;
        if (index < 0) index = points.Length - 1;
        StartCoroutine(MoveToPosition(points[index].position));
    }
    
    private IEnumerator MoveToPosition(Vector3 newPosition)
    {
        isMoving = true;
        while (transform.position != newPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = newPosition; 
        isMoving = false;
        OnMoveComplete?.Invoke();
    }
    
    public event Action OnMoveComplete;
    
    private void UpdateColor()
    {
        if (transform.position.z < 1.5)
        {
            ChangeColor(white);
            return;
        }
        if (transform.position.z < 2)
        {
            ChangeColor(blueLight);
            return;
        }
        if (transform.position.z < 2.7)
        {
            ChangeColor(blueMedium);
            return;
        }
        ChangeColor(blueDark);
    }

    private void ChangeColor(Color newColor)
    {
        sprite.color = newColor;
        mesh.materials[0].color = newColor;
        text.color = newColor;
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Labeler : MonoBehaviour
{
    TextMeshPro label;
    Vector2Int cords = new Vector2Int();
    GridManager gridmanager;

    private void Awake()
    {
        gridmanager = FindObjectOfType<GridManager>();
        label = GetComponentInChildren<TextMeshPro>();

        DisplayCords();
    }

    private void Update()
    {
        DisplayCords();
        transform.name = cords.ToString();
    }

    private void DisplayCords()
    {
        if (!gridmanager) { return; }
        cords.x = Mathf.RoundToInt(transform.position.x + 0.5f / gridmanager.UnityGridSize);
        cords.y = Mathf.RoundToInt(transform.position.z + 0.5f / gridmanager.UnityGridSize);

        //label.text = $"{cords.x}, {cords.y}";
    }
}

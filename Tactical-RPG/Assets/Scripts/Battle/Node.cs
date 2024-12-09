using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node")]
public class Node : ScriptableObject
{
    public string type;
    public int moveCost;
}

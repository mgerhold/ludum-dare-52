using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GrowthDurationsScriptableObject",
    order = 1)]
public class GrowthDurationsScriptableObject : SerializedScriptableObject {
    public Dictionary<PlantType, float> durations;
}
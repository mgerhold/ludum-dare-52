using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PricesScriptableObject", order = 1)]
public class PricesScriptableObject : SerializedScriptableObject {
    public Dictionary<PlantType, long> prices;
}
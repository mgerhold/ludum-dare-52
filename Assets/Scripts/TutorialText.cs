using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialText : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI text = null;

    public string Text {
        get => text.text;
        set => text.text = value;
    }
}
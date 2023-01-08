using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour {
    [SerializeField] private AudioClipPlayer failureSoundPlayer = null;
    [SerializeField] private AudioClipPlayer moneySoundPlayer = null;
    public Transform handPosition = null;

    public void PlayFailureSound() {
        failureSoundPlayer.PlaySound();
    }

    public void PlayMoneySound() {
        moneySoundPlayer.PlaySound();
    }
}
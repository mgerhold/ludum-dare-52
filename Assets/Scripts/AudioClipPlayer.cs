using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] audioClips = null;
    [SerializeField] private AudioSource audioSource = null;

    public void PlaySound() {
        audioSource.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }
}
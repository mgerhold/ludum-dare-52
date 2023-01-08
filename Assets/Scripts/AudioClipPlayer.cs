using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] audioClips = null;
    [SerializeField] private AudioSource audioSource = null;
    private int _nextIndex = 0;

    public void PlaySound() {
        audioSource.clip = audioClips[_nextIndex];
        _nextIndex = (_nextIndex + 1) % audioClips.Length;
        audioSource.Play();
    }
}
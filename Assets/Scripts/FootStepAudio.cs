using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepAudio : MonoBehaviour
{
    public AudioSource footStepAudio;
    private void OnTriggerEnter(Collider other)
    {
        footStepAudio.pitch = Random.Range(0.8f, 1.1f);
        footStepAudio.volume = Random.Range(0.4f, 0.8f);
        footStepAudio.Play();
    }
}

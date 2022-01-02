using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepAudio : MonoBehaviour
{
    public AudioSource footStepAudio;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("FootStep");
        footStepAudio.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundCollection : MonoBehaviour
{
    [SerializeField] private string name;
    [SerializeField] private float volume = 1f;
    [SerializeField] private AudioClip[] clips;

    public AudioClip GetRandomSound()
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public string GetName()
    {
        return name;
    }

    public float GetVolume()
    {
        return volume;
    }
}

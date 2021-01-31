using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private SoundCollection[] _soundCollections;
    private void Awake()
    {
        _soundCollections = GetComponentsInChildren<SoundCollection>();
    }

    public void PlaySound(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.PlayOneShot(clip);
        Destroy(source, clip.length);
    }
    
    public void PlayRandomSound(string name)
    {
        foreach (SoundCollection collection in _soundCollections)
        {
            if (collection.GetName() == name)
            {
                AudioClip clip = collection.GetRandomSound();
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.PlayOneShot(clip, collection.GetVolume());
                Destroy(source, clip.length);
            }
        }
    }
    
    public void PlaySpatialSound(AudioClip clip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.PlayOneShot(clip);
        source.spatialBlend = 1;
        source.volume = 0.5f;
        Destroy(source, clip.length);
    }
}

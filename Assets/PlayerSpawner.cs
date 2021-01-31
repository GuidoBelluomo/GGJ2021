using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private AudioClip gameStartClip;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip zombieStart;
    public void PlaySound(AudioClip clip, bool destroy = true, string name = "Audio Source")
    {
        GameObject gameObject = new GameObject(name);
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;

        if (destroy)
        { 
            Destroy(gameObject, clip.length);
            source.PlayOneShot(clip);
        }
        else
        {
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
    }

    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(2.1f);
        PlaySound(zombieStart);
        yield return new WaitForSeconds(zombieStart.length / 2);
        playerPrefab.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        PlaySound(gameMusic, false, "Level Music");
        Destroy(gameObject);
        yield return null;
    }

    public void PlayIntro()
    {
        PlaySound(gameStartClip);
    }
    
    public void SpawnPlayer()
    {
        StartCoroutine(StartMusic());
    }
}

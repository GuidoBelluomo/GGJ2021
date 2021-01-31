using System;
using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects.Pickups.Items
{
    public class LevelEnd : MonoBehaviour
    {
        [SerializeField] private AudioClip onorevole;
        [SerializeField] private AudioClip victoryJingle;
        [SerializeField] private string nextLevelName;
        private bool _pickedUp;
        
        public void PlaySound(AudioClip clip)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.playOnAwake = false;
            source.Play();
            Destroy(source, clip.length);
        }
        
        IEnumerator Pickup()
        {
            _pickedUp = true;
            GetComponent<SpriteRenderer>().enabled = false;
            PlaySound(victoryJingle);
            yield return new WaitForSeconds(3f);
            PlaySound(onorevole);
            yield return new WaitForSeconds(onorevole.length);
            ScreenFade.FadeOut();
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(nextLevelName);
            yield return null;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_pickedUp) return;
            
            Destroy(GameObject.Find("Level Music"));
            if (other.gameObject.layer == 3)
            {
                StartCoroutine(Pickup());
            }
        }
    }
}

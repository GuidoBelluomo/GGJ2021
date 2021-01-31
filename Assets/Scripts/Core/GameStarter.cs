using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameStarter : MonoBehaviour
    {
        private IEnumerator StartGame(string sceneName)
        {
            ScreenFade.FadeOut();
            yield return new WaitForSeconds(2.5f);
            SceneManager.LoadScene(sceneName);
            yield return null;
        }
    }
}

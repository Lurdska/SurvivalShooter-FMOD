using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace CompleteProject
{
    public class GameOverManager : MonoBehaviour
    {
        public PlayerHealth playerHealth; // Reference to the player's health.
        Animator anim;

        private bool hasRestarted = false;

        void Awake()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (playerHealth.currentHealth <= 0 && !hasRestarted)
            {
                anim.SetTrigger("GameOver");
                hasRestarted = true;
                StartCoroutine(RestartGameAfterDelay());
            }
        }

        IEnumerator RestartGameAfterDelay()
        {
            yield return new WaitForSeconds(2.5f); // Ajustá este valor si tu animación es más larga

            FindObjectOfType<MusicControl>()?.StopMusic();

            SceneManager.LoadScene("Survival-Shooter-Fmod-Demo");
        }
    }
}
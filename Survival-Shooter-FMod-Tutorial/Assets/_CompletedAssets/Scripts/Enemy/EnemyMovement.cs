using UnityEngine;
using System.Collections;

namespace CompleteProject
{
    public class EnemyMovement : MonoBehaviour
    {
        Transform player;
        PlayerHealth playerHealth;
        EnemyHealth enemyHealth;
        UnityEngine.AI.NavMeshAgent nav;

        [SerializeField] private string footstepSoundPath;
        [SerializeField] private float footstepInterval = 0.5f;

        private bool isWalking = false;

        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealth = GetComponent<EnemyHealth>();
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

        void Update()
        {
            if (enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
            {
                nav.SetDestination(player.position);

                if (nav.velocity.magnitude > 0.1f && !isWalking)
                {
                    isWalking = true;
                    InvokeRepeating(nameof(PlayFootstep), 0f, footstepInterval);
                }
                else if (nav.velocity.magnitude <= 0.1f && isWalking)
                {
                    isWalking = false;
                    CancelInvoke(nameof(PlayFootstep));
                }
            }
            else
            {
                nav.enabled = false;

                // 👇 cancelamos pasos si muere
                if (isWalking)
                {
                    isWalking = false;
                    CancelInvoke(nameof(PlayFootstep));
                }
            }
        }

        void PlayFootstep()
        {
            FMODUnity.RuntimeManager.PlayOneShot(footstepSoundPath, transform.position);
        }
    }
}
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using FMODUnity;

namespace CompleteProject
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 6f; // velocidad del player

        // --- FOOTSTEPS ---
        [Header("FMOD Footsteps")]
        public StudioEventEmitter footstepEmitter;
        public float stepDistance = 0.40f;         // metros por paso (0.35–0.5 aprox)
        public float minSpeedForStep = 0.10f;      // umbral para no disparar en casi quieto
        public float groundedRayDist = 0.30f;      // chequeo suelo

        Vector3 movement;
        Animator anim;
        Rigidbody playerRigidbody;
#if !MOBILE_INPUT
        int floorMask;
        float camRayLength = 100f;
#endif

        // acumulador de distancia y posición previa
        Vector3 prevPos;
        float distAccum = 0f;

        void Awake()
        {
#if !MOBILE_INPUT
            floorMask = LayerMask.GetMask("Floor");
#endif
            anim = GetComponent<Animator>();
            playerRigidbody = GetComponent<Rigidbody>();
            prevPos = transform.position;
        }

        void FixedUpdate()
        {
            float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float v = CrossPlatformInputManager.GetAxisRaw("Vertical");

            Move(h, v);
            Turning();
            Animating(h, v);

            FootstepTick(h, v); // dispara pasos según movimiento
        }

        void Move(float h, float v)
        {
            movement.Set(h, 0f, v);
            movement = movement.normalized * speed * Time.deltaTime;
            playerRigidbody.MovePosition(transform.position + movement);
        }

        void Turning()
        {
#if !MOBILE_INPUT
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit floorHit;

            if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
            {
                Vector3 playerToMouse = floorHit.point - transform.position;
                playerToMouse.y = 0f;
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);
                playerRigidbody.MoveRotation(newRotatation);
            }
#else
            Vector3 turnDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X"), 0f, CrossPlatformInputManager.GetAxisRaw("Mouse Y"));
            if (turnDir != Vector3.zero)
            {
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;
                playerToMouse.y = 0f;
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
        }

        void Animating(float h, float v)
        {
            bool walking = h != 0f || v != 0f;
            anim.SetBool("IsWalking", walking);
        }

        // ---------------- FOOTSTEPS ----------------
        void FootstepTick(float h, float v)
        {
            // distancia horizontal recorrida este frame (usamos delta de posición real)
            Vector3 pos = transform.position;
            Vector3 delta = pos - prevPos;
            delta.y = 0f;
            float horizDelta = delta.magnitude;

            // velocidad horizontal aproximada
            float horizSpeed = horizDelta / Mathf.Max(Time.deltaTime, 0.0001f);

            bool moving = (h != 0f || v != 0f) && horizSpeed > minSpeedForStep;

            // grounded: raycast corto hacia abajo contra capa Floor
            bool grounded = Physics.Raycast(transform.position + Vector3.up * 0.05f,
                                            Vector3.down,
                                            groundedRayDist + 0.05f,
#if !MOBILE_INPUT
                                            floorMask
#else
                                            ~0
#endif
                                            );

            if (moving && grounded && footstepEmitter != null)
            {
                distAccum += horizDelta;
                if (distAccum >= stepDistance)
                {
                    footstepEmitter.Play(); // dispara el evento de paso (one-shot)
                    distAccum = 0f;
                }
            }
            else
            {
                // si se detiene, no sigue acumulando
                distAccum = 0f;
            }

            prevPos = pos;
        }
    }
}

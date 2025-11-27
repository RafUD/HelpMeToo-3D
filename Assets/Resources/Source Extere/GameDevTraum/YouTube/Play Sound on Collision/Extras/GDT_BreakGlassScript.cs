//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace GameDevTraum
//{
//    namespace YouTube
//    {
//        namespace PlaySoundOnCollision
//        {

//            public class GDT_BreakGlassScript : MonoBehaviour
//            {

//                AudioManager audioManager;

//                private void Awake()
//                {
//                    audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

//                }


//                public GameObject normalGlass;
//                public GameObject destroyedGlassParent;




//                private void OnTriggerEnter(Collider other)
//                {
//                    if (other.CompareTag("Player"))
//                    {
//                        destroyedGlassParent.SetActive(true);
//                        normalGlass.SetActive(false);
//                        audioManager.PlaySFX(audioManager.mirrorBreak);
//                    }

//                }

//            }
//        }
//    }
//}

using System.Collections;
using UnityEngine;

namespace GameDevTraum
{
    namespace YouTube
    {
        namespace PlaySoundOnCollision
        {
            public class GDT_BreakGlassScript : MonoBehaviour
            {
                [Header("Glass Objects")]
                public GameObject normalGlass;
                public GameObject destroyedGlassParent;

                [Header("Physics Settings")]
                public float explosionForce = 300f;
                public float explosionRadius = 2f;
                public float glassLifetime = 5f;

                private AudioManager audioManager;
                private bool isBroken = false;

                private void Awake()
                {
                    GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
                    if (audioObj != null)
                        audioManager = audioObj.GetComponent<AudioManager>();
                }

                private void Start()
                {
                    // Ensure broken glass is hidden at start
                    if (destroyedGlassParent != null)
                        destroyedGlassParent.SetActive(false);
                }

                private void OnTriggerEnter(Collider other)
                {
                    if (other.CompareTag("Player") && !isBroken)
                    {
                        BreakGlass(other);
                    }
                }

                private void BreakGlass(Collider playerCollider)
                {
                    isBroken = true;

                    // Activate broken glass
                    if (destroyedGlassParent != null)
                        destroyedGlassParent.SetActive(true);

                    // Deactivate intact glass
                    if (normalGlass != null)
                        normalGlass.SetActive(false);

                    // Play break sound
                    if (audioManager != null && audioManager.mirrorBreak != null)
                        audioManager.PlaySFX(audioManager.mirrorBreak);

                    // Apply physics and ignore player collisions
                    if (destroyedGlassParent != null)
                    {
                        ApplyGlassPhysics(playerCollider);
                    }

                    // Clean up after delay
                    StartCoroutine(CleanupGlass());
                }

                private void ApplyGlassPhysics(Collider playerCollider)
                {
                    Rigidbody[] glassPieces = destroyedGlassParent.GetComponentsInChildren<Rigidbody>();
                    Vector3 explosionCenter = transform.position;

                    foreach (Rigidbody piece in glassPieces)
                    {
                        if (piece != null)
                        {
                            // CRITICAL: Make glass ignore player collision
                            Collider pieceCollider = piece.GetComponent<Collider>();
                            if (pieceCollider != null && playerCollider != null)
                            {
                                Physics.IgnoreCollision(pieceCollider, playerCollider, true);
                            }

                            // Apply explosion force
                            piece.AddExplosionForce(explosionForce, explosionCenter, explosionRadius);

                            // Add random spin
                            piece.AddTorque(Random.insideUnitSphere * explosionForce * 0.1f);
                        }
                    }
                }

                private IEnumerator CleanupGlass()
                {
                    yield return new WaitForSeconds(glassLifetime);

                    // Destroy broken glass pieces
                    if (destroyedGlassParent != null)
                        Destroy(destroyedGlassParent);

                    // Destroy this parent object
                    Destroy(gameObject, 0.1f);
                }
            }
        }
    }
}

/* ============================================
   SETUP INSTRUCTIONS
   ============================================

HIERARCHY STRUCTURE:
--------------------
GlassWindow (this script here)
├─ Box Collider (Is Trigger: ✓ CHECKED)
├─ NormalGlass (assign to normalGlass field)
│  └─ Glass mesh (visible)
└─ DestroyedGlassParent (assign to destroyedGlassParent field)
   ├─ Piece1
   ├─ Piece2
   └─ Piece3


EACH BROKEN PIECE NEEDS:
-------------------------
1. Rigidbody
   - Use Gravity: ✓
   - Mass: 0.5 (adjust for weight)
   - Drag: 0.5 (slight air resistance)

2. Mesh Collider
   - Convex: ✓ CHECKED
   - Is Trigger: ✗ UNCHECKED (must collide with ground!)

3. Material
   - Physics Material with some bounce (optional)


SETTINGS TO ADJUST:
-------------------
- explosionForce: 300 (higher = more dramatic break)
- explosionRadius: 2 (how far force spreads)
- glassLifetime: 5 (seconds before pieces disappear)


IMPORTANT FIXES APPLIED:
-------------------------
✓ Glass pieces now IGNORE player collision (no pushing!)
✓ Glass pieces still fall and hit ground properly
✓ Explosion force applied for realistic break
✓ Random spin added to pieces
✓ Automatic cleanup after delay
✓ Only breaks once (isBroken check)
✓ Null checks for safety

*/

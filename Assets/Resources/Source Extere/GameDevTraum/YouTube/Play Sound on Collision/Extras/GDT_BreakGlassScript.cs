using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTraum
{
    namespace YouTube
    {
        namespace PlaySoundOnCollision
        {

            public class GDT_BreakGlassScript : MonoBehaviour
            {

                AudioManager_3D audioManager;

                private void Awake()
                {
                    audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager_3D>();

                }


                public GameObject normalGlass;
                public GameObject destroyedGlassParent;




                private void OnTriggerEnter(Collider other)
                {
                    if (other.CompareTag("Player"))
                    {
                        normalGlass.SetActive(false);
                        destroyedGlassParent.SetActive(true);
                        audioManager.PlaySFX(audioManager.mirrorBreak);
                    }

                }

            }
        }
    }
}

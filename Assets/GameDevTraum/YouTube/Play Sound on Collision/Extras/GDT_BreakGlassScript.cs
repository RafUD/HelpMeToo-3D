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

                AudioManager audioManager;

                private void Awake()
                {
                    audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

                }


                public GameObject normalGlass;
                public GameObject destroyedGlassParent;




                private void OnTriggerEnter(Collider other)
                {
                    if (other.CompareTag("Player"))
                    {
                        destroyedGlassParent.SetActive(true);
                        normalGlass.SetActive(false);
                        audioManager.PlaySFX(audioManager.mirrorBreak);
                    }
                        
                }

            }
        }
    }
}


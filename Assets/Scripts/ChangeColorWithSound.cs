using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FfmpegUnity.Sample
{
    public class ChangeColorWithSound : MonoBehaviour
    {
        IEnumerator Start()
        {
            var renderer = GetComponentInChildren<Renderer>();
            var audioSource = GetComponentInChildren<AudioSource>();

            renderer.material.color = Color.red;

            for (; ; )
            {
                yield return new WaitForSeconds(1f);

                renderer.material.color = Color.red;
                //audioSource.Play();

                yield return new WaitForSeconds(1f);

                renderer.material.color = Color.yellow;
                //audioSource.Play();

                yield return new WaitForSeconds(1f);

                renderer.material.color = Color.blue;
                //audioSource.Play();

                yield return new WaitForSeconds(1f);

                renderer.material.color = Color.green;
                //audioSource.Play();

            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameControll
{
    public class ParticleHook : MonoBehaviour
    {
        ParticleSystem[] particles;

        public void Init()
        {
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        public void Emit(int v)
        {
            if (particles == null)
                return;
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Emit(v);
            }
        }
    }
}


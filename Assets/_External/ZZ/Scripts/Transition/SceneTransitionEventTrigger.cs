﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ZZ
{
    [ExecuteInEditMode]
    public class SceneTransitionEventTrigger : MonoBehaviour
    {
        public float alpha;

        [SerializeField]
        SceneTransition target;

        [SerializeField]
        Image image;

        void Awake()
        {
            FindTarget();
        }

        void Update()
        {
            image.color = new Color(0, 0, 0, alpha);

            FindTarget();
        }

        void OnValidate()
        {
            FindTarget();
        }

        void FindTarget()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (target == null)
                {
                    target = FindObjectOfType<SceneTransition>();
                }

                if (image == null)
                {
                    image = GetComponent<Image>();
                }
            }
#endif
        }

        public void OnFadedIn()
        {
            target.OnFadedIn();
        }

        public void OnFadedOut()
        {
            target.OnFadedOut();
        }
    }
}

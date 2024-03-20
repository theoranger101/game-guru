using System;
using UnityEngine;

namespace Project_2.Scripts
{
    public class MovingPlatform : MonoBehaviour
    {
        public Rigidbody Rigidbody;

        private void OnEnable()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Rigidbody.AddForce(Vector3.right, ForceMode.Force);
        }
    }
}
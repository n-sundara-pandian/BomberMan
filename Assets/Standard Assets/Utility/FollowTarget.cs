using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public float MinHeight = 8;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);
        public List<Transform> TargetList = new List<Transform>();
        public Transform p1;
        public Transform p2;
        Vector3 velocity = Vector3.zero;
        void UpdateTargetTransform()
        {
            Vector3 MeanPoint = (p2.localPosition - p1.localPosition) /2 + p1.localPosition;
            Vector3 dist = p2.position - p1.position;
            float height = dist.magnitude * 1.25f;
            offset = MeanPoint;
            if (height < MinHeight) height = MinHeight;
            offset.y = height;
            target.position = MeanPoint;
        }
        private void LateUpdate()
        {
            UpdateTargetTransform();
            transform.position = Vector3.SmoothDamp(transform.position, offset, ref velocity,0.1f); 
        }
    }
}

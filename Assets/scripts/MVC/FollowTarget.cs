using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        float MinHeight = 8;
        Vector3 offset = new Vector3(0f, 7.5f, 0f);
        Transform player1 = null;
        Transform player2 = null;
        Vector3 velocity = Vector3.zero;
        public void SetPlayer(int player_no, Transform t)
        {
            if (player_no == 1) player1 = t;
            else player2 = t;
        }
        void UpdateTargetTransform()
        {
            if (player1 == null || player1 == null) return;
            Vector3 MeanPoint = (player2.localPosition - player1.localPosition) /2 + player1.localPosition;
            Vector3 dist = player2.position - player1.position;
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

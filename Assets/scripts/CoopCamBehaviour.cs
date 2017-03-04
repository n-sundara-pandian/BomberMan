using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopCamBehaviour : MonoBehaviour {
    public List<Transform> TargetList = new List<Transform>();
    Vector3 targetPos;
    float speed = 5.0f;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    // Use this for initialization
    void Start () {
        InvokeRepeating("UpdatePos", 0.1f, 0.1f);
	}
	
	// Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

/*        Vector3 dir = targetPos - transform.position;
        if (dir.sqrMagnitude > 2)
        {
            dir.Normalize();
            transform.position += dir * Time.deltaTime * speed;
        }*/
    }
	void UpdatePos () {
	}

}

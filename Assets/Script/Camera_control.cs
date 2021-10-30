using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_control : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 offset;
    [SerializeField] public Transform target;
    public float distanceUp = 15f;
    public float distanceAway = 10f;
    public float smooth = 2f;//位置平滑移动值
    public float camDepthSmooth = 5f;
    private float Speed = 20f;
    private float Offset = 20f;
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = offset + target.position;
        transform.LookAt(target.position);
        transform.RotateAround(target.position, target.up, target.rotation.eulerAngles.y);
    }
}

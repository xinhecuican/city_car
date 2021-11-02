using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_control : MonoBehaviour
{
	//// Start is called before the first frame update
	//private Vector3 offset;
	//[SerializeField] public Transform target;
	//public float distanceUp = 15f;
	//public float distanceAway = 10f;
	//public float smooth = 2f;//λ��ƽ���ƶ�ֵ
	//public float camDepthSmooth = 5f;
	//private float Speed = 20f;
	//private float Offset = 20f;
	//void Start()
	//{
	//    offset = transform.position - target.position;
	//}

	//// Update is called once per frame
	//void Update()
	//{
	//    transform.position = offset + target.position;
	//    transform.LookAt(target.position);
	//    transform.RotateAround(target.position, target.up, target.rotation.eulerAngles.y);
	//}
	public Transform car;
	public float distance = 6.4f;
	public float height = 1.4f;
	public float rotationDamping = 3.0f;
	public float heightDamping = 2.0f;
	public float zoomRatio = 1f;
	public float defaultFOV = 60f;

	private Vector3 rotationVector;

	void LateUpdate()
	{
		float wantedAngle = rotationVector.y;
		float wantedHeight = car.position.y + height;
		float myAngle = transform.eulerAngles.y;
		float myHeight = transform.position.y;

		myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping * Time.deltaTime);
		myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping * Time.deltaTime);

		Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);
		transform.position = car.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		Vector3 temp = transform.position; //temporary variable so Unity doesn't complain
		temp.y = myHeight;
		transform.position = temp;
		transform.LookAt(car);
		var dv = Input.GetAxis("Mouse ScrollWheel");
		if (height - dv > 0f)
		{
			distance -= dv;
			height -= dv;
		}

	}

    void FixedUpdate()
    {
        Vector3 localVelocity = car.InverseTransformDirection(car.GetComponent<Rigidbody>().velocity);
        if (localVelocity.z < -0.1f)
        {
            Vector3 temp = rotationVector; //because temporary variables seem to be removed after a closing bracket "}" we can use the same variable name multiple times.
			temp.y = car.eulerAngles.y;
            rotationVector = temp;
        }
        else
        {
            Vector3 temp = rotationVector;
            temp.y = car.eulerAngles.y;
            rotationVector = temp;
        }
        float acc = car.GetComponent<Rigidbody>().velocity.magnitude;
        GetComponent<Camera>().fieldOfView = defaultFOV + acc * zoomRatio * Time.deltaTime;  //he removed * Time.deltaTime but it works better if you leave it like this.
    }
}

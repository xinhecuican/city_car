using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private MeshRenderer[] wheels;
    private WheelCollider[] colliders;
    private float horizontal;
    private float vertical;
    public float maxDigree = 30f;
    public float maxMotor = 3300f;
    public float maxSpeed = 200f;
    public float spring = 1000f;
    private float current_torque;
    private float maxBrake = float.MaxValue;
    private Rigidbody rigidBody;
    private float old_rotation;

    private int CurrentGear;
    private int EngineRPM;
    private int MaxEngineRPM = 3000;
    private float[] GearRatio =  {4.06f, 2.3f, 1.59f, 1.25f, 1f, 0.8f};
    private float MinEngineRPM = 1000;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GameObject.FindObjectOfType<Rigidbody>();
        wheels = GameObject.FindGameObjectWithTag("CarWheels").GetComponentsInChildren<MeshRenderer>();
        colliders = GameObject.FindGameObjectWithTag("WheelColliders").GetComponentsInChildren<WheelCollider>();
        Vector3 vec = rigidBody.centerOfMass;
        vec.y -= 0.1f;
        rigidBody.centerOfMass = vec;
        current_torque = 50f;
    }

    private void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Move();
    }
    void Move()
    {
        TorqueControl();
        for (int j = 0; j < 4; j++)//轮胎旋转
        {
            wheels[j].transform.localRotation = Quaternion.Euler(colliders[j].rpm / 60 * 360,
                colliders[j].steerAngle, wheels[j].transform.localRotation.z);
        }
        SteerHelper();
        colliders[0].attachedRigidbody.AddForce(-transform.up * 100f *
                                                         colliders[0].attachedRigidbody.velocity.magnitude);
        CapSpeed();
        TranctionControl();
    }

    private void TranctionControl()
    {
        WheelHit wheelHit;
        colliders[2].GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);
        colliders[3].GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);
    }

    private void AdjustTorque(float forwardSlip)
    {
        //当向前滑动距离超过阈值后，就说明轮胎过度滑转，则减少牵引力，以降低转速。
        if (forwardSlip >= 30f && current_torque >= 0)
        {
            current_torque -= 5;
        }
        else
        {
            current_torque += 5;
            if (current_torque > 100f)
            {
                current_torque = 100f;
            }
        }
    }

    private void SteerHelper()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            colliders[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(old_rotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - old_rotation) * 0.3f;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rigidBody.velocity = velRotation * rigidBody.velocity;
        }
        old_rotation = transform.eulerAngles.y;
    }

    private void CapSpeed()
    {
        float speed = rigidBody.velocity.magnitude;
        speed *= 3.6f;
        if (speed > maxSpeed)
            rigidBody.velocity = (maxSpeed / 3.6f) * rigidBody.velocity.normalized;
    }

    private void antiRoll(int left, int right)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;
        //计算两侧轮胎在不同情况下的悬挂系数
        bool groundedL = colliders[left].GetGroundHit(out hit);
        if (groundedL)//着地
            travelL = (-colliders[left].transform.InverseTransformPoint(hit.point).y - colliders[left].radius)
                / colliders[left].suspensionDistance;

        bool groundedR = colliders[right].GetGroundHit(out hit);
        if (groundedR)
            travelR = (-colliders[right].transform.InverseTransformPoint(hit.point).y - colliders[right].radius)
                / colliders[right].suspensionDistance;

        //计算平衡杆刚度系数
        float antiRollForce = (travelL - travelR) * spring;

        //向两侧的轮胎分配力
        if (groundedL)
            rigidBody.AddForceAtPosition(colliders[left].transform.up * -antiRollForce, colliders[left].transform.position);
        if (groundedR)
            rigidBody.AddForceAtPosition(colliders[right].transform.up * antiRollForce, colliders[right].transform.position);
    }

    private void TorqueControl()
    {
        for (int i = 0; i < 4; i++)//汽车动力
        {
            if (i < 2)
            {
                colliders[i].steerAngle = horizontal * maxDigree;

            }
            else
            {
                float brake = 0f;
                if ((vertical * maxMotor < 0 && colliders[i].rpm > 5) || (vertical * maxMotor > 0 && colliders[i].rpm < -5))
                {
                    brake = maxBrake;
                }
                colliders[i].motorTorque = current_torque * vertical / 2;
                colliders[i].brakeTorque = brake;
            }
        }
    }

    void ShiftGears()
    {
        int AppropriateGear = CurrentGear;
        if (EngineRPM >= MaxEngineRPM)
        {
            for (int i = 0; i < GearRatio.Length; i++)
            {
                if (colliders[0].rpm * GearRatio[i] < MaxEngineRPM)
                {
                    AppropriateGear = i;
                    break;
                }
            }
            CurrentGear = AppropriateGear;
        }

        if (EngineRPM <= MaxEngineRPM)
        {
            for (int j = GearRatio.Length - 1; j >= 0; j--)
            {
                if (colliders[0].rpm * GearRatio[j] > MinEngineRPM)
                {
                    AppropriateGear = j;
                    break;
                }
            }
            CurrentGear = AppropriateGear;
        }
    }
}

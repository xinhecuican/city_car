using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarCtrl : MonoBehaviour
{
    public Image image;
    private class Lights
    {
        public Transform light_transform;
        public Light FrontL;
        public Light FrontR;
        public void front_visibility()
        {
            bool enable = FrontL.enabled;
            FrontL.enabled = !enable;
            FrontR.enabled = !enable;
        }
    }
    private Lights lights;
    private MeshRenderer[] wheels;
    private WheelCollider[] colliders;
    private float horizontal;
    private float vertical;
    public float baseDigree = 20f;
    private float shiftDigree = 0f;
    public float maxMotor = 30000f;
    public float maxSpeed = 200f;
    public float spring = 1000f;
    private float current_torque;
    private float maxBrake = 10000000f;
    private Rigidbody rigidBody;
    private float old_rotation;

    private int CurrentGear;
    private float EngineRPM;
    private int MaxEngineRPM = 1500;
    private float[] GearRatio =  {3f, 2.3f, 1.59f, 4.02f};
    private float MinEngineRPM = 600;
    private float energy;
    private ParticleSystem particle;
    private float Energy{ 
        get{return energy;}
        set
        {
            if (value > 100)
            {
                energy = 100;
            }
            else if (value < 0)
            {
                energy = 0;
            }
            else
            {
                energy = value;
            }
        }
    }
    private bool is_accelerate;
    private bool space_enter;
    private float motor_maintain;

    private void Awake()
    {
        Cursor.visible = false;
        Energy = 0;
        is_accelerate = false;
        space_enter = false;
        lights = new Lights();
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Pause();
        wheels = GameObject.FindGameObjectWithTag("CarWheels").GetComponentsInChildren<MeshRenderer>();
        colliders = GameObject.FindGameObjectWithTag("WheelColliders").GetComponentsInChildren<WheelCollider>();
        lights.light_transform = GameObject.Find("Lights").transform;
        lights.FrontL = lights.light_transform.GetChild(0).GetComponent<Light>();
        lights.FrontR = lights.light_transform.GetChild(1).GetComponent<Light>();
        lights.FrontR.enabled = false;
        lights.FrontL.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GameObject.FindObjectOfType<Rigidbody>();
        Vector3 vec = rigidBody.centerOfMass;
        vec.y -= 0.1f;
        rigidBody.centerOfMass = vec;
        current_torque = maxMotor;
        CurrentGear = 0;
    }

    private void Update()
    {
        for (int i = 0; i < 4; i++)//让车轮跟随碰撞器的移动而移动
        {
            Vector3 pos;
            Quaternion quat;
            colliders[i].GetWorldPose(out pos, out quat);
            wheels[i].transform.position = pos;
            wheels[i].transform.rotation = quat;
        }

        if(Input.GetKeyDown(KeyCode.G))//前照灯
        {
            lights.front_visibility();
        }

        if(Input.GetKeyDown(KeyCode.Space))//漂移
        {
            space_enter = true;
            for(int i=0; i<4; i++)
            {
                if (i < 2)
                {
                    WheelFrictionCurve curve = colliders[i].sidewaysFriction;
                    curve.extremumValue = 0.8f;
                    colliders[i].sidewaysFriction = curve;
                }
                else
                {
                    WheelFrictionCurve curve = colliders[i].sidewaysFriction;
                    curve.extremumValue = 0.3f;
                    colliders[i].sidewaysFriction = curve;
                }
            }
            Energy += Time.deltaTime;
            shiftDigree = 30f;
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            space_enter = false;
            for (int i = 0; i < 4; i++)
            {
                WheelFrictionCurve curve = colliders[i].sidewaysFriction;
                curve.extremumValue = 1f;
                colliders[i].sidewaysFriction = curve;
            }
            shiftDigree = 0f;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))//加速
        {
            if(Energy <= 0)
            {
                particle.Stop();
                is_accelerate = false;
            }
            else
            {
                particle.Play();
                is_accelerate = true;
            }
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            particle.Stop();
            is_accelerate = false;
        }
        if(is_accelerate)
        {
            Energy -= Time.deltaTime * 15;
            if(Energy <= 0.1f)
            {
                particle.Stop();
                is_accelerate = false;
            }
            motor_maintain = 1200f;
            
        }
        else
        {
            motor_maintain = 0f;
        }
        if(space_enter && Math.Abs(colliders[0].steerAngle) > 5 && rigidBody.velocity.magnitude >= 5f)
        {
            Energy += Time.deltaTime * 20;
        }
        image.fillAmount = Energy / 100f;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Move();
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.tag == "Car")
        {
            rigidBody.angularVelocity = new Vector3(-rigidBody.angularVelocity.x * 0.5f, rigidBody.angularVelocity.y * 0.5f, -rigidBody.angularVelocity.z * 0.5f);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y * 0.5f, rigidBody.velocity.z);
        }

    }
    void Move()
    {
        TorqueControl();
        SteerHelper();
        //施加向下的力，增加稳定性
        colliders[0].attachedRigidbody.AddForce(-transform.up * 100f *
                                                         colliders[0].attachedRigidbody.velocity.magnitude);
        CapSpeed();//控制最大速度
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
            if (current_torque > maxMotor)
            {
                current_torque = maxMotor;
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
        ShiftGears();//改变档位
        for (int i = 0; i < 4; i++)//汽车动力
        {
            if (i < 2)
            {
                colliders[i].steerAngle = horizontal * (baseDigree + shiftDigree + rigidBody.velocity.magnitude / 5);//转向
            }
            else
            {
                float brake = 0f;
                //如果运动方向和想要运动的方向相反则刹车
                if ((vertical * maxMotor < 0 && colliders[i].rpm > 5) || (vertical * maxMotor > 0 && colliders[i].rpm < -5))
                {
                    brake = maxBrake + motor_maintain;
                }
                colliders[i].motorTorque = current_torque / GearRatio[CurrentGear] * vertical + motor_maintain;
                colliders[i].brakeTorque = brake;
            }
        }
    }

    void ShiftGears()
    {
        EngineRPM = (colliders[0].rpm + colliders[1].rpm) / 2 * GearRatio[CurrentGear];
        int AppropriateGear = CurrentGear;
        if(colliders[0].rpm < -5 && vertical < 0)//倒车
        {
            CurrentGear = 3;
            return;
        }
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
        }
        CurrentGear = AppropriateGear;
    }
}

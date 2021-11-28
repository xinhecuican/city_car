using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    private WheelCollider[] colliders;
    private float time; //拖尾持续时间
    private class Renders
    {
        public TrailRenderer renderL;
        public TrailRenderer renderR;
    }
    private Renders renders;
    bool isTrail; //拖尾效果是否持续中
    bool space_enter;
    private float horizontal;
    private float vertical;
    private float hide_time;
    private bool begin_hide;
    // Start is called before the first frame update
    void Start()
    {
        hide_time = 0f;
        begin_hide = false;
        renders = new Renders();
        TrailRenderer[] temp = GameObject.Find("trails").GetComponentsInChildren<TrailRenderer>();
        renders.renderL = temp[0];
        renders.renderR = temp[1];
        renders.renderL.forceRenderingOff = true;
        renders.renderR.forceRenderingOff = true;
        colliders = GameObject.FindGameObjectWithTag("WheelColliders").
            GetComponentsInChildren<WheelCollider>();
        
    }

    private bool need_draw()
    {
        if(!colliders[2].isGrounded || !colliders[3].isGrounded 
            || Math.Abs(colliders[0].steerAngle) < 5 || !space_enter)
        {
            return false;
        }
        return true;
    }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            space_enter = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            space_enter = false;
        }
        if (need_draw())
        {
            begin_hide = false;
            renders.renderL.forceRenderingOff = false;
            renders.renderR.forceRenderingOff = false;
        }
        else
        {
            begin_hide = true;
            
        }
        if(begin_hide)
        {
            hide_time += Time.deltaTime;
            if(hide_time > 1f)
            {
                begin_hide = false;
                renders.renderL.forceRenderingOff = true;
                renders.renderR.forceRenderingOff = true;
            }
        }
        else
        {
            hide_time = 0f;
        }
    }
}

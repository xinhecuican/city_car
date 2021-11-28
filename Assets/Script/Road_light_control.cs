using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road_light_control : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject[] high_nlamp;
    GameObject[] high_slamp;
    GameObject[] high_wlamp;
    GameObject[] high_elamp;
    GameObject[] short_wlamp;
    GameObject[] short_elamp;
    GameObject[] short_slamp;
    GameObject[] short_nlamp;
    public Vector3 h_red;
    public Vector3 h_green;
    public Vector3 h_yellow;
    public Vector3 s_red;
    public Vector3 s_green;
    public Vector3 s_yellow;
    public float intensity = 0.01f;
    public float angle = 120f;
    public float range = 30f;
    public GameObject prefab;
    private enum direction{North, West, South, Eash, NS, WE};
    private List<Light> HN_light;
    private List<Light> HW_light;
    private List<Light> HS_light;
    private List<Light> HE_light;
    private List<Light> N_light;
    private List<Light> W_light;
    private List<Light> E_light;
    private List<Light> S_light;
    private int now_time;
    private bool is_greeen;

    void Start()
    {
        HN_light = new List<Light>();
        HS_light = new List<Light>();
        HW_light = new List<Light>();
        HE_light = new List<Light>();
        N_light = new List<Light>();
        W_light = new List<Light>();
        E_light = new List<Light>();
        S_light = new List<Light>();
        high_nlamp = GameObject.FindGameObjectsWithTag("HNLamp");
        high_elamp = GameObject.FindGameObjectsWithTag("HELamp");
        high_wlamp = GameObject.FindGameObjectsWithTag("HWLamp");
        high_slamp = GameObject.FindGameObjectsWithTag("HSLamp");
        short_wlamp = GameObject.FindGameObjectsWithTag("SWLamp");
        short_elamp = GameObject.FindGameObjectsWithTag("SELamp");
        short_slamp = GameObject.FindGameObjectsWithTag("SSLamp");
        short_nlamp = GameObject.FindGameObjectsWithTag("SNLamp");
        is_greeen = false;
        for(int i=0; i<high_nlamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = high_nlamp[i].transform;
            Light light = _object.GetComponent<Light>();
            light.spotAngle = angle;
            light.transform.localPosition = h_green;
            light.color = new Color(0, 255, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            HN_light.Add(light);
        }
        for(int i=0; i<high_slamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = high_slamp[i].transform;
            Light light = _object.GetComponent<Light>();
            light.spotAngle = angle;
            light.transform.localPosition = h_green;
            light.color = new Color(0, 255, 0);
            _object.transform.rotation = Quaternion.Euler(0, -180f, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            HS_light.Add(light);
        }
        for(int i=0; i<high_elamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = high_elamp[i].transform;
            Light light = _object.GetComponent<Light>();

            light.spotAngle = angle;
            light.transform.localPosition = h_red;
            light.color = new Color(255, 0, 0);
            _object.transform.rotation = Quaternion.Euler(0, 90f, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            HE_light.Add(light);
        }
        for (int i = 0; i < high_wlamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = high_wlamp[i].transform;
            Light light = _object.GetComponent<Light>();

            light.spotAngle = angle;
            light.transform.localPosition = h_red;
            light.color = new Color(255, 0, 0);
            _object.transform.rotation = Quaternion.Euler(0, -90f, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            HW_light.Add(light);
        }
        for (int i=0; i<short_nlamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = short_nlamp[i].transform;
            Light light = _object.GetComponent<Light>();
            light.spotAngle = angle;
            light.transform.localPosition = s_red;
            light.transform.localRotation = Quaternion.Euler(0, 90f, 0);
            //light.transform.rotation = Quaternion.Euler(0, -90f, 0);
            light.color = new Color(255, 0, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            N_light.Add(light);
        }
        for(int i=0; i<short_slamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = short_slamp[i].transform;
            Light light = _object.GetComponent<Light>();
            light.spotAngle = angle;
            light.transform.localPosition = s_red;
            light.transform.rotation = Quaternion.Euler(0, 0, 0);
            light.color = new Color(255, 0, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            S_light.Add(light);
        }
        for(int i=0; i<short_elamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = short_elamp[i].transform;
            Light light = _object.GetComponent<Light>();
            light.spotAngle = angle;
            light.transform.localPosition = s_green;
            light.transform.rotation = Quaternion.Euler(0, -90f, 0);
            light.color = new Color(0, 255, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            E_light.Add(light);
        }
        for(int i=0; i<short_wlamp.Length; i++)
        {
            GameObject _object = GameObject.Instantiate(prefab) as GameObject;
            _object.transform.parent = short_wlamp[i].transform;
            Light light = _object.GetComponent<Light>();
            light.spotAngle = angle;
            light.transform.localPosition = s_green;
            light.transform.rotation = Quaternion.Euler(0, 90f, 0);
            light.color = new Color(0, 255, 0);
            light.range = range;
            light.intensity = intensity;
            light.type = LightType.Spot;
            W_light.Add(light);
        }
        InvokeRepeating("change_light", 10, 10);
    }

    // Update is called once per frame
    void change_light()
    {
        if(is_greeen)
        {
            
            for (int i = 0; i < HN_light.Count; i++)
            {
                HN_light[i].transform.localPosition = h_green;
                HN_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < HS_light.Count; i++)
            {
                HS_light[i].transform.localPosition = h_green;
                HS_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < HE_light.Count; i++)
            {
                HE_light[i].transform.localPosition = h_red;
                HE_light[i].color = new Color(255, 0, 0);
            }
            for (int i = 0; i < HW_light.Count; i++)
            {
                HW_light[i].transform.localPosition = h_red;
                HW_light[i].color = new Color(255, 0, 0);
            }
            for (int i = 0; i < N_light.Count; i++)
            {
                N_light[i].transform.localPosition = s_red;
                N_light[i].color = new Color(255, 0, 0);
            }
            for (int i = 0; i < S_light.Count; i++)
            {
                S_light[i].transform.localPosition = s_red;
                S_light[i].color = new Color(255, 0, 0);
            }
            for (int i = 0; i < E_light.Count; i++)
            {
                E_light[i].transform.localPosition = s_green;
                E_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < W_light.Count; i++)
            {
                W_light[i].transform.localPosition = s_green;
                W_light[i].color = new Color(0, 255, 0);
            }
            is_greeen = false;
        }
        else
        {
            for (int i = 0; i < HN_light.Count; i++)
            {
                HN_light[i].transform.localPosition = h_red;
                HN_light[i].color = new Color(255, 0, 0);
            }
            for (int i = 0; i < HS_light.Count; i++)
            {
                HS_light[i].transform.localPosition = h_red;
                HS_light[i].color = new Color(255, 0, 0);
            }
            for (int i = 0; i < HE_light.Count; i++)
            {
                HE_light[i].transform.localPosition = h_green;
                HE_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < HW_light.Count; i++)
            {
                HW_light[i].transform.localPosition = h_green;
                HW_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < N_light.Count; i++)
            {
                N_light[i].transform.localPosition = s_green;
                N_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < S_light.Count; i++)
            {
                S_light[i].transform.localPosition = s_green;
                S_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < E_light.Count; i++)
            {
                E_light[i].transform.localPosition = s_red;
                E_light[i].color = new Color(0, 255, 0);
            }
            for (int i = 0; i < W_light.Count; i++)
            {
                W_light[i].transform.localPosition = s_red;
                W_light[i].color = new Color(0, 255, 0);
            }
            is_greeen = true;
        }    
    }
}

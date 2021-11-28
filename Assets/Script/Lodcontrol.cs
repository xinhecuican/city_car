using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Lodcontrol : MonoBehaviour
{
    public float fade0 = 0.5f;
    public float fade1 = 0.25f;

    private void Start()
    {
        LODGroup[] groups = GetComponentsInChildren<LODGroup>();
        for(int i=0; i<groups.Length; i++)
        {
            LOD[] lods = groups[i].GetLODs();
            if(lods.Length >= 1)
                lods[0].screenRelativeTransitionHeight = fade0;
            if(lods.Length >= 2)
                lods[1].screenRelativeTransitionHeight = fade1;
            groups[i].SetLODs(lods);
            groups[i].fadeMode = LODFadeMode.CrossFade;
        }
    }


}

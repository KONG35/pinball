using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameObjectManager : MonoBehaviour
{
    public Transform[] spawnTrArr;

    [SerializeField]
    private Transform dontTouchGroupTr;
    [SerializeField]
    private Transform MustTouchGroupTr;

    //[SerializeField]
    //private Color32 dontTouchClor;
    //[SerializeField]
    //private Color32 mustTouchClor;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public float dissolveTime =5f;
    
    void Awake(){
        dissolveTime += Time.time;
    } 

    void Update(){
        if (Time.time >= dissolveTime){
            Destroy(gameObject);
        }
    }
}

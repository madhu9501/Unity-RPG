using MyRPG;
using UnityEngine;

namespace MyRPG{
    
    [System.Serializable]
    public class PlayerScanner
    {
        public float meleeDetectionRange = 4f; 
        public float detectionAngle = 90f;
        public float detectionRange = 10f;


        public PlayerMovement Detect( Transform detector ){
            if(PlayerMovement.Instance == null){ return null; }

            Vector3 toPlayer = PlayerMovement.Instance.transform.position - detector.position;
            toPlayer.y = 0f;
            
            if(toPlayer.magnitude <= detectionRange){
                if((Vector3.Dot(toPlayer.normalized, detector.forward) > Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad )) || (toPlayer.magnitude <= meleeDetectionRange)){
                    return PlayerMovement.Instance;
                }
            }

            return null;
        }
    }
}




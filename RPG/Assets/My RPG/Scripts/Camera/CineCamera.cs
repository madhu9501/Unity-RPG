using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace MyRPG{
    public class CineCamera : MonoBehaviour
    {
        [SerializeField]
        private CinemachineFreeLook freeLookCam;

        public CinemachineFreeLook PlayerCineCamera
        {
            get{ return freeLookCam; }
        }

        void Update()
        {
            if(Input.GetMouseButtonDown(1)){
                freeLookCam.m_XAxis.m_MaxSpeed = 400;
                freeLookCam.m_YAxis.m_MaxSpeed = 10;
            }
            
            if(Input.GetMouseButtonUp(1)){
                freeLookCam.m_XAxis.m_MaxSpeed = 0;
                freeLookCam.m_YAxis.m_MaxSpeed = 0;
            }
            
        }
    }

}


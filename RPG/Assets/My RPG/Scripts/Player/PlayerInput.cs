using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyRPG{
    public class PlayerInput : MonoBehaviour
    {
        // Singleton Design
        private static PlayerInput _instance;       
        public static PlayerInput Instance {get{return _instance;}}
        void Awake()
        {
            _instance = this;
        }

        private Vector3 _moveIp;
        public Vector3 MoveIp{
            get
            { 
                if(IsPlayerInputBlocked){
                    return Vector3.zero;
                }
                return _moveIp;
            }
        }
        private bool _isAttack;
        public bool IsAttack{ get{ return !IsPlayerInputBlocked && _isAttack; }}
        public bool IsMoveing{get{ return !Mathf.Approximately(_moveIp.magnitude, 0); }}
        public bool IsPlayerInputBlocked;

        private Collider _optionClickTgt;
        public Collider OptionClickTgt{get{ return _optionClickTgt; }}


        void Update()
        {
            _moveIp.Set(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            bool isLeftMouseClick = Input.GetMouseButtonDown(0);
            bool isRightMouseClick = Input.GetMouseButtonDown(1);

            if(isLeftMouseClick && !_isAttack){
                if(IsPointerOverUI()){
                    return;
                }
                StartCoroutine(TriggerAttack());
            }

            if(isRightMouseClick){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool hasHit = Physics.Raycast( ray, out RaycastHit hit);

                if(hasHit){
                    StartCoroutine(TriggerOptionTgt( hit.collider));
                }
            }
        }

        // Trigger Attacking
        private IEnumerator TriggerAttack(){
            _isAttack = true;
            yield return new WaitForSeconds(0.02f);
            _isAttack = false;
        }

        // Trigger Dialouge
        private IEnumerator TriggerOptionTgt(Collider other){
            _optionClickTgt = other;
            yield return new WaitForSeconds(0.02f);
            _optionClickTgt = null;
        }

        // Deisable attack when cursor is over UI
        private bool IsPointerOverUI(){
            var eventData = new PointerEventData(EventSystem.current){
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            return results.Count > 0;
        }
    }

}


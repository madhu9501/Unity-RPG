using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG
{
    public partial class Damageable : MonoBehaviour
    {
        
        [Range(0f,360f)]
        public float hitAngle = 360f;
        public LayerMask playerActionReciver;
        public float invulnerabilityTime = 0.5f;
        public int maxHitPoint;
        public int experience;
        public int CurrentHitPoints{ get; private set; }
        private bool isInvulnerable = false;
        private float timeSinceLastHit= 0f;

        // MonoBehaviour => scripts 
        public List<MonoBehaviour> onDamageMsgRecivers; 

        void Awake(){            
            SetInitialHealth();
            
            if( 0 != (playerActionReciver.value & 1 << gameObject.layer))
            {
                onDamageMsgRecivers.Add(FindObjectOfType<QuestManager>());
                onDamageMsgRecivers.Add(FindObjectOfType<PlayerStats>());
            }


        }

        public void SetInitialHealth(){
            CurrentHitPoints = maxHitPoint;
        }

        private void Update(){
            if(isInvulnerable){
                timeSinceLastHit += Time.deltaTime;
                if(timeSinceLastHit > invulnerabilityTime){
                    isInvulnerable = false;
                    timeSinceLastHit = 0;
                }
            }
        }

        public void ApplyDamage(DamageMessage data){
            if(CurrentHitPoints <= 0 || isInvulnerable){
                return;
            }
            Vector3 positionToDamager = data.damageSource.transform.position - transform.position;
            positionToDamager.y = 0f;

            if(Vector3.Angle(transform.forward, positionToDamager) > hitAngle * 0.5f){
                return;
            }
            isInvulnerable = true;

            CurrentHitPoints -= data.amount;
            MessageType messageType = CurrentHitPoints <=0 ? MessageType.DEAD : MessageType.DAMAGED;

            for(int i=0; i < onDamageMsgRecivers.Count; i++){
                var reciver = onDamageMsgRecivers[i] as IMessageReciver;
                reciver.OnMessageRecive(messageType, this, data);   
            }



        }



    #if UNITY_EDITOR

    private void OnDrawGizmosSelected(){
        Color colour = new Color(0,0,7f,0.4f);
        UnityEditor.Handles.color = colour;

        Vector3 rotatedForward = Quaternion.AngleAxis( -hitAngle * 0.5f, transform.up) * transform.forward;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotatedForward, hitAngle, 1f);

    }

    #endif
    }
    
}


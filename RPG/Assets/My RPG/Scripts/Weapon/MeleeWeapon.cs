using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG{
    public class MeleeWeapon : MonoBehaviour
    {
        [System.Serializable]
        public class AttackPoint{
            public Transform rootTransform;
            public Vector3 offset;
            public float radius;
        }

        public AttackPoint[] attackPoints;
        public LayerMask targetLayer;
        public RandomAudio swingAudio;
        public RandomAudio impactAudio;


        public int damage;
        private bool isAttack;
        private Vector3[] originAttackPos;
        private RaycastHit[] raycastHitsCache = new RaycastHit[32];

        private GameObject swordOwner;

        private void FixedUpdate(){

            if(isAttack){
                for(int i=0; i < attackPoints.Length; i++ ){
                    AttackPoint ap = attackPoints[i];
                    Vector3 worldPos = ap.rootTransform.position + ap.rootTransform.TransformVector(ap.offset);
                    Vector3 attackVector = (worldPos - originAttackPos[i]).normalized;
                    Ray ray = new Ray(worldPos, attackVector);
                    Debug.DrawRay(worldPos, attackVector, Color.red, 4f);

                    int contacts = Physics.SphereCastNonAlloc(ray, ap.radius, raycastHitsCache, attackVector.magnitude, ~0, QueryTriggerInteraction.Ignore);
                    for(int j =0; j<contacts; j++){
                        Collider collider = raycastHitsCache[j].collider;
                        if(collider != null ){
                            CheckDamage(collider);
                        }
                    }
                    originAttackPos[0]= worldPos;
                }
                
            }

        }

        public void SetTargetLayer(LayerMask targetMask){
            targetLayer = targetMask;
        }

        public void SetOwner(GameObject owner){
            swordOwner = owner;
        }

        private void CheckDamage(Collider other){
            if((targetLayer.value & (1 << other.gameObject.layer)) == 0){
                return;
            }

            Damageable damageable = other.GetComponent<Damageable>();
            if (damageable != null){
                Damageable.DamageMessage data;
                data.amount= damage;
                data.damager = this;
                data.damageSource = swordOwner;
                if(impactAudio != null){
                    impactAudio.PlayAudioClips();
                }
                damageable.ApplyDamage(data);
            }
        }

        public void BeginAttack(){
            if(impactAudio != null){
                swingAudio.PlayAudioClips();
            }

            isAttack = true;
            originAttackPos = new Vector3[attackPoints.Length];

            for(int i=0; i < attackPoints.Length; i++ ){
                AttackPoint ap = attackPoints[i];
                originAttackPos[i] = ap.rootTransform.position + ap.rootTransform.TransformDirection(ap.offset);
            }
        }

        public void EndAttack(){
            isAttack = false;
        }













    #if UNITY_EDITOR

    private void OnDrawGizmosSelected(){
        foreach(AttackPoint attackpoint in attackPoints){
            
            if(attackpoint.rootTransform != null){

                Vector3 worldPos = attackpoint.rootTransform.TransformVector(attackpoint.offset);

                Gizmos.color = new Color(1,1,1, 0.6f);
                Gizmos.DrawSphere(attackpoint.rootTransform.position + worldPos, attackpoint.radius );

            }
        }
    }

    #endif

    }

}


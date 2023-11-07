using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyRPG {

    public class BanditBehaviour : MonoBehaviour, IMessageReciver, IAttackAnimeListener
    {
        // Components
        public MeleeWeapon meleeWeapon;
        private PlayerMovement followTarget;
        private EnemyController enemyController;
        public PlayerScanner playerScanner;

        public bool HasFollowTarget{
            get{ return followTarget != null; }
        }


        private float timeToStopPursuit = 2f;
        private float timeLostTarget = 0f;
        private Vector3 originPos;
        private Quaternion originRot;
        private float attackDist = 1.1f;


        // Animator
        private readonly int inPursuitHash = Animator.StringToHash("InPursuit");
        private readonly int nearBaseHash = Animator.StringToHash("NearBase");
        private readonly int attackHash = Animator.StringToHash("Attack");
        private readonly int isHurtHash = Animator.StringToHash("IsHurt");
        private readonly int isDeadHash = Animator.StringToHash("IsDead");
        
        
        
        private void Awake(){
            enemyController = GetComponent<EnemyController>();
            originPos = transform.position;
            originRot = transform.rotation;
            meleeWeapon.SetOwner(gameObject);
            
        }
        void Start(){
            meleeWeapon.SetTargetLayer(1 << PlayerMovement.Instance.gameObject.layer);
        }
        
        private void Update(){
            if(PlayerMovement.Instance.IsRespawning){
                GoToOriginPos();
                CheckIfNearBase();
                return;
            }
            GaurdPos();            
        }

        private void GaurdPos(){
            var detectedTarget = playerScanner.Detect(transform);
            bool hasDetectedTarget = detectedTarget != null;
            
            if(hasDetectedTarget) { followTarget = detectedTarget; }

            if(HasFollowTarget)
            {
                AttackOrFollowTaregt();
                if(hasDetectedTarget){
                    timeLostTarget = 0f;
                }else
                {
                    StopPuruite();
                }
            }
            CheckIfNearBase();

        }

        private void AttackOrFollowTaregt(){

            Vector3 toTarget = followTarget.transform.position - transform.position;
            if(toTarget.magnitude <= attackDist)
            {
                AttackTarget(toTarget);
            }else
            {
                FollowTarget();
            }
        }

        private void AttackTarget(Vector3 toTarget){
                Quaternion targetRot = Quaternion.LookRotation(toTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 600 * Time.fixedDeltaTime);
                enemyController.StopFollowTarget();
                enemyController.Animator.SetTrigger(attackHash);
        }

        private void FollowTarget(){
            enemyController.Animator.SetBool(inPursuitHash, true);
            enemyController.FollowTarget(followTarget.transform.position);
        }


        private void StopPuruite(){

            timeLostTarget += Time.deltaTime;
            if(timeLostTarget >= timeToStopPursuit)
            {
                followTarget = null;
                enemyController.Animator.SetBool(inPursuitHash, false);
                StartCoroutine(WaitBeforeReturn());
            }  
        }

        private IEnumerator WaitBeforeReturn(){
            yield return new WaitForSeconds(2f);
            enemyController.FollowTarget(originPos);
        }

        private void CheckIfNearBase(){
            Vector3 toBase = originPos - transform.position;
            toBase.y=0 ;
            bool nearBase = toBase.magnitude < 0.01f;
            enemyController.Animator.SetBool( nearBaseHash, nearBase);

            if(nearBase)
            {
                Quaternion tgtRot = Quaternion.RotateTowards(transform.rotation, originRot, 600 * Time.fixedDeltaTime);
                transform.rotation = tgtRot;    
            }

        }

        // public void OnMessageRecive(MessageType type, Damageable sender, Damageable.DamageMessage msg){
        public void OnMessageRecive(MessageType type, object sender, object msg){

            switch(type){
                
                case MessageType.DEAD:
                    OnDead();
                    break;             
                case MessageType.DAMAGED:
                    OnReciveDamage();
                    break;
                default:
                    break;
            }
            
        }

        private void OnDead(){
            // enemyController.StopFollowTarget();
            enemyController.Animator.SetTrigger(isDeadHash);
        }

        private void OnReciveDamage(){
            enemyController.Animator.SetTrigger(isHurtHash);
        }

        public void MeleeAttackStart()
        {
            meleeWeapon.BeginAttack();
        }

        public void MeleeAttackStop()
        {
            meleeWeapon.EndAttack();
        }

        private void GoToOriginPos(){
            followTarget = null;
            enemyController.Animator.SetBool( inPursuitHash, false);
            enemyController.FollowTarget(originPos);
        }





#if UNITY_EDITOR
        private void OnDrawGizmosSelected(){
            Color colour = new Color(0,0,7f,0.4f);
            UnityEditor.Handles.color = colour;

            Vector3 rotatedForward = Quaternion.Euler(0, - playerScanner.detectionAngle* 0.5f, 0) * transform.forward;
            UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotatedForward, playerScanner.detectionAngle, playerScanner.detectionRange);
            
            UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, rotatedForward, 360, playerScanner.meleeDetectionRange);
            
        }


#endif
    }

}




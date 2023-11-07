using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyRPG {
    public class EnemyController : MonoBehaviour, IAttackAnimeListener
    {
        
        private Animator animator;
        public Animator Animator {get{return animator;}}
        public float speedModifier;
        private NavMeshAgent navMeshAgent;
        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        void OnAnimatorMove(){
            if(navMeshAgent.enabled) {
                navMeshAgent.speed = (animator.deltaPosition / Time.fixedDeltaTime).magnitude * speedModifier;
            }

        }

        public bool FollowTarget(Vector3 position){
            if(!navMeshAgent.enabled){ navMeshAgent.enabled = true; }
            return navMeshAgent.SetDestination(position);
        }

        public void StopFollowTarget(){
            navMeshAgent.enabled = false;
        }

        public void MeleeAttackStart()
        {
        }

        public void MeleeAttackStop()
        {
        }
    }
}


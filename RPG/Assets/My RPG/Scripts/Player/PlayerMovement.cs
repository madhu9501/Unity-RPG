using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRPG{

    public class PlayerMovement : MonoBehaviour, IAttackAnimeListener, IMessageReciver
    {
        //Components
        [SerializeField]
        private CineCamera _cineCamera;
        private CharacterController _characterController; 
        private PlayerInput _playerInput;
        private Animator _animator;
        private static PlayerMovement _instance;
        public MeleeWeapon MeleeWeapon;
        public Transform AttackHand;
        private HudManager _hudManager;
        private Damageable _damageable;
        public RandomAudio FootfallSounds;

        //Player Movement
        private Vector3 _moveDir;
        public float MoveSpeed;
        private float _fwdSpeed;
        private float _maxFwdSpeed;
        private float _accPlayer;
        private float _decPlayer;
        private Vector3 _originalPos;

        // Gravity
        private float _gravityFoce = 0.3f;
        private float _gravity;

        //Player Roattion
        private Vector3 _camDir;
        private Quaternion _playerRot;
        public float MaxRotSpeed = 1200f;
        public float MinRotSpeed = 800f;
        private Quaternion _originalRot;

        // Animator Hash
        private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private readonly int _meleeAttackHash = Animator.StringToHash("MeleeAttack");
        private readonly int _deathHash = Animator.StringToHash("Death");
        private readonly int _footfallHash = Animator.StringToHash("Footfall");
                    
        private readonly int _blockInputHash = Animator.StringToHash("BlockInput");

        private AnimatorStateInfo _currentStateInfo;
        private AnimatorStateInfo _nextStateInfo;
        private bool _isAnimatorTransitioning;

        private bool _isRespawning;
        public bool IsRespawning{get{return _isRespawning;}}

        public static PlayerMovement Instance {
            get { return _instance; }
        } 

        void Awake(){
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _animator = GetComponent<Animator>();
            _hudManager = FindObjectOfType<HudManager>(); 
            _damageable = GetComponent<Damageable>();
            _cineCamera = Camera.main.GetComponent<CineCamera>();
            
            _instance =this;
            _gravity = -_gravityFoce;
            _hudManager.SetMaxHealth(_damageable.maxHitPoint);
            _originalPos = transform.position;
            _originalRot = transform.rotation;
        }

        void Start(){
            _maxFwdSpeed = 10f;
            _accPlayer = 20f;
            _decPlayer = 30f;
        }

        void Update(){
            _moveDir = _playerInput.MoveIp.normalized;
        }
        
        void FixedUpdate()
        {
            PlayerMove();
            PlayerRotation();

            if(_playerInput.IsMoveing){
                float rotSpeed = Mathf.Lerp(MaxRotSpeed, MinRotSpeed, MoveSpeed/_fwdSpeed);
                _playerRot = Quaternion.RotateTowards(transform.rotation, _playerRot, rotSpeed * Time.fixedDeltaTime);
                transform.rotation = _playerRot;
            }

            _animator.ResetTrigger(_meleeAttackHash);
            if(_playerInput.IsAttack){
                _animator.SetTrigger(_meleeAttackHash);
            }

            PlaySprintAudio();
        }

        // private void OnAnimatorMove(){
        //     if(_isRespawning){return;}

        //     Vector3 movement = _animator.deltaPosition;
        //     movement += _gravity * Vector3.up;
        //     _characterController.Move(movement);
        // }

        private void PlayerMove(){
            _fwdSpeed = _moveDir.magnitude * _maxFwdSpeed;
            float acc = _playerInput.IsMoveing ? _accPlayer : _decPlayer;
            MoveSpeed = Mathf.MoveTowards(MoveSpeed, _fwdSpeed, Time.fixedDeltaTime * acc);

            _animator.SetFloat(_moveSpeedHash, MoveSpeed);
        }

        private void PlayerRotation(){
            // returna vector 3 direction 
            _camDir = Quaternion.Euler(0, _cineCamera.PlayerCineCamera.m_XAxis.Value, 0) * Vector3.forward;

            Quaternion tgtRot;
            // for back direction rotation
            if(Mathf.Approximately(Vector3.Dot(_moveDir, Vector3.forward), -1)){
                tgtRot = Quaternion.LookRotation(-_camDir);                
            }else{
                Quaternion moveRot = Quaternion.FromToRotation(Vector3.forward, _moveDir);
                tgtRot = Quaternion.LookRotation(moveRot * _camDir);                  
            }

            _playerRot = tgtRot;
        }

///////////////////////////// Combat system /////////////////////////////
        public void MeleeAttackStart(){
            if(MeleeWeapon != null){
                MeleeWeapon.BeginAttack();                
            }
        }

        public void MeleeAttackStop(){
            if(MeleeWeapon != null){
                MeleeWeapon.EndAttack();              
            }            
        }

        internal void UseItemFrom(InventorySlots slot)
        {
            if(MeleeWeapon != null){
                if(MeleeWeapon.name == slot.itemName){ return;}
                else{
                    Destroy(MeleeWeapon.gameObject);
                }
            }
            MeleeWeapon = Instantiate(slot.itemPrefab, transform).GetComponent<MeleeWeapon>(); 
            MeleeWeapon.GetComponent<FixedUpdateFollow>().SetFollowee(AttackHand);
            MeleeWeapon.name = slot.itemName;
            MeleeWeapon.SetOwner(gameObject);
        }

        public void OnMessageRecive(MessageType type, object sender, object msg)
        {
            if(type == MessageType.DAMAGED){
                _hudManager.SetHealth((sender as Damageable).CurrentHitPoints );
            }

            if(type == MessageType.DEAD){
                _isRespawning = true;
                _animator.SetTrigger(_deathHash);
                _hudManager.SetHealth(0);
            }
        }

        private void CacheAnimatioState(){
            _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            _nextStateInfo = _animator.GetNextAnimatorStateInfo(0);
            _isAnimatorTransitioning = _animator.IsInTransition(0);
        }

        private void UpdateInputBloking(){
            bool inputBlocked = _currentStateInfo.tagHash == _blockInputHash && !_isAnimatorTransitioning;
            inputBlocked |= _nextStateInfo.tagHash == _blockInputHash;
            _playerInput.IsPlayerInputBlocked = inputBlocked;
        }

        public void StartRespawn(){
            transform.position = _originalPos;
            transform.rotation = _originalRot;
            _hudManager.SetHealth(_damageable.maxHitPoint);
            _damageable.SetInitialHealth();
        }

        public void FinishRespawn(){
            _isRespawning = false;
        }

        private void PlaySprintAudio(){
            float footfallCurve = _animator.GetFloat(_footfallHash);
            
            if(footfallCurve > 0.01f && !FootfallSounds.isPlaying && FootfallSounds.canPlay){
                FootfallSounds.isPlaying = true;
                FootfallSounds.canPlay = false;
                FootfallSounds.PlayAudioClips();
            }else if(FootfallSounds.isPlaying){
                FootfallSounds.isPlaying = false;
            }else if(footfallCurve < 0.01f && !FootfallSounds.canPlay){
                FootfallSounds.canPlay = true;
            }
        }
    }

}
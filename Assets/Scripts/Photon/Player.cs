using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static NinjaTools.Utils;
public enum AnimState {
    Idle = 0, Walk = 1, Run = 2, Jump = 3, Fall = 4, Land = 5, Crouch = 6, CrouchWalk = 7
}

public class Player : NetworkBehaviour {

    [SerializeField] PhysxBall _prefabPhysxBall;
    [SerializeField] Ball _prefabBall;
    [SerializeField] ParticleSystem _shotFX;
    [SerializeField] Animator _anim;

    [SerializeField] float ballInitImpulse = 10f;
    [SerializeField] float ballSpawnDelay = 0.25f;

    //public Material _material;

    [Networked]
    public bool spawned { get; set; }

    AnimState _animState;
    [Networked]
    public int animState { get; set; }

    [Networked]
    public float velocity { get; set; }

    private ChangeDetector _changeDetector;
    public override void Spawned() {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    [Networked] TickTimer delay { get; set; }

    NetworkCharacterController _cc;
    Vector3 _forward = Vector3.forward;

    public float movementSpeed = 5f; 
    private TMP_Text _messages;
    void Awake() {
        //_material = GetComponentInChildren<MeshRenderer>().material;
        _cc = GetComponent<NetworkCharacterController>();
        _forward = transform.forward;
    }
    private void Update() {
        if(Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R)) {
            RPC_SendMessage("Hey Mate");
        }
    }
    public override void FixedUpdateNetwork() {

        if (GetInput(out NetworkInputData data)) {
            data.direction.Normalize();

            _cc.braking = _cc.Grounded ? 20 : 0;

            _cc.Move(movementSpeed * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0) {
                _forward = data.direction;
            }

            if (!HasStateAuthority || !delay.ExpiredOrNotRunning(Runner)) {
                return;
            }
            var ccVelocity = _cc.Velocity;
            if(animState!=(int)AnimState.Idle && ccVelocity.sqrMagnitude <= 0.14f) {
                animState = (int)AnimState.Idle;
                _animState = AnimState.Idle;
            }
            if (animState!=(int)AnimState.Run && ccVelocity.sqrMagnitude >= 0.15f) {
                animState = (int)AnimState.Run;
                _animState = AnimState.Run;
            }
            if(animState==(int)AnimState.Run) {
                velocity = ccVelocity.magnitude;
            }
            logd("FixedUpdateNetwork", $"ccVelocity={ccVelocity} magnitude={ccVelocity.magnitude} AnimState={animState}", true, 1f);

            HandlePlayerInput(data);
        }
    }

    public void HandlePlayerInput(NetworkInputData data) {
        
        if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0)) {
            SpawnBall();
        }
        if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1)) {
            SpawnPhysxBall();
        }
        if (data.buttons.IsSet(NetworkInputData.KEYCODE_SPACE)) {
            _cc.Jump();
        }
    }

    public override void Render() {
        var logId = "Render";
        foreach (var change in _changeDetector.DetectChanges(this)) {
            logd(logId, $"Change: {change}");
            switch (change) {
                case nameof(spawned):
                    //_material.color = Color.white;
                    _shotFX.Play();
                    break;
                case nameof(animState):
                    logd(logId, $"animState={_animState.ToString()}");
                    _anim.Play(_animState.ToString());
                    break;

            }
        }
        //_material.color = Color.Lerp(_material.color, Color.blue, Time.deltaTime);
    }
    public void SpawnPhysxBall() {
        var logId = "SpawnPhysxBall";
        logd(logId, "Spawning PhysxBall");
        delay = TickTimer.CreateFromSeconds(Runner, ballSpawnDelay);
        Runner.Spawn(_prefabPhysxBall,
                    transform.position + _forward,
                    Quaternion.LookRotation(_forward),
                    Object.InputAuthority,
                    (runner, o) => {
                        o.GetComponent<PhysxBall>().Init(ballInitImpulse * _forward);
                    });
        spawned = !spawned;
    }
    public void SpawnBall() {
        var logId = "SpawnBall";
        logd(logId, "Spawning Ball");
        delay = TickTimer.CreateFromSeconds(Runner, ballSpawnDelay);
        Runner.Spawn(_prefabBall,
                    transform.position + _forward,
                    Quaternion.LookRotation(_forward),
                    Object.InputAuthority,
                    (runner, o) => {
                        o.GetComponent<Ball>().Init();
                    });
        spawned = !spawned;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default) {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource) {
        if (_messages == null)
            _messages = FindObjectOfType<TMP_Text>();

        if (messageSource == Runner.LocalPlayer) {
            message = $"You said: {message}\n";
        } else {
            message = $"Some other player said: {message}\n";
        }

        _messages.text += message;
    }
}
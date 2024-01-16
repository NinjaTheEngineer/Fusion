using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour {

    [SerializeField] PhysxBall _prefabPhysxBall;
    [SerializeField] Ball _prefabBall;

    [SerializeField] float ballInitImpulse = 10f;
    [SerializeField] float ballSpawnDelay = 0.25f;


    [Networked] TickTimer delay { get; set; }

    NetworkCharacterController _cc;
    Vector3 _forward = Vector3.forward;

    public float movementSpeed = 5f;
    void Awake() {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = transform.forward;
    }
    public override void FixedUpdateNetwork() {

        if (GetInput(out NetworkInputData data)) {
            data.direction.Normalize();
            _cc.Move(movementSpeed * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0) {
                _forward = data.direction;
            }

            if (!HasStateAuthority || !delay.ExpiredOrNotRunning(Runner)) {
                return;
            }

            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0)) {
                delay = TickTimer.CreateFromSeconds(Runner, ballSpawnDelay);
                Runner.Spawn(_prefabBall,
                             transform.position + _forward,
                             Quaternion.LookRotation(_forward),
                             Object.InputAuthority,
                             (runner, o) => {
                                 o.GetComponent<Ball>().Init();
                             });
            }

            if(data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1)) {
                delay = TickTimer.CreateFromSeconds(Runner, ballSpawnDelay);
                Runner.Spawn(_prefabPhysxBall,
                             transform.position + _forward,
                             Quaternion.LookRotation(_forward),
                             Object.InputAuthority,
                             (runner, o) => {
                                 o.GetComponent<PhysxBall>().Init(ballInitImpulse * _forward);
                             });
            }
        }
    }
}
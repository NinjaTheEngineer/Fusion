using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour {

    [SerializeField] private Ball _prefabBall;
    
    [Networked] private TickTimer delay { get; set; }
    
    private NetworkCharacterController _cc;
    Vector3 _forward = Vector3.forward;
    
    public float movementSpeed = 5f;
    private void Awake() {
        _cc = GetComponent<NetworkCharacterController>();
        _forward = transform.forward;
    }
    public override void FixedUpdateNetwork() {

        if (GetInput(out NetworkInputData data)) {
            data.direction.Normalize();
            _cc.Move(movementSpeed * data.direction * Runner.DeltaTime);

            if(data.direction.sqrMagnitude>0) {
                _forward = data.direction;
            }
            
            if(HasStateAuthority && delay.ExpiredOrNotRunning(Runner) && data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0)) {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(
                             _prefabBall,
                             transform.position + _forward,
                             Quaternion.LookRotation(_forward),
                             Object.InputAuthority,
                             (runner, o) => {
                                 o.GetComponent<Ball>().Init();
                             });
            }
        }
    }
}
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour {
    public float movementSpeed = 5f;
    private NetworkCharacterController _cc;
    private void Awake() {
        _cc = GetComponent<NetworkCharacterController>();
    }
    public override void FixedUpdateNetwork() {
        if (GetInput(out NetworkInputData data)) {
            data.direction.Normalize();
            _cc.Move(movementSpeed * data.direction * Runner.DeltaTime);
        }
    }
}
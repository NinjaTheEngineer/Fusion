using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysxBall : NetworkBehaviour {
    [Networked] private TickTimer life { get; set; }
    [SerializeField] float lifeTime = 5.0f;
    public void Init(Vector3 forward) {
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork() {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
    }
}

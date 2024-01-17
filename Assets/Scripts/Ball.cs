using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    public float speed = 5f;
    public float lifeTime = 5f;
    [Networked] private TickTimer life { get; set; }
    public override void FixedUpdateNetwork() {
        transform.position += speed * transform.forward * Runner.DeltaTime;
        if(life.Expired(Runner)) {
            Runner.Despawn(Object);
        }
    }
    public void Init() {
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }
}

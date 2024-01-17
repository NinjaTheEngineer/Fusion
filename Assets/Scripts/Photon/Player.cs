using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NinjaTools.Utils;
public class Player : NetworkBehaviour {

    [SerializeField] PhysxBall _prefabPhysxBall;
    [SerializeField] Ball _prefabBall;

    [SerializeField] float ballInitImpulse = 10f;
    [SerializeField] float ballSpawnDelay = 0.25f;

    public Material _material;

    [Networked]
    public bool spawned { get; set; }
    private ChangeDetector _changeDetector;
    public override void Spawned() {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    [Networked] TickTimer delay { get; set; }

    NetworkCharacterController _cc;
    Vector3 _forward = Vector3.forward;

    public float movementSpeed = 5f;
    void Awake() {
        _material = GetComponentInChildren<MeshRenderer>().material;
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
                SpawnBall();
            }
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1)) {
                SpawnPhysxBall();
            }
        }
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
    public override void Render() {
        var logId = "Render";
        foreach (var change in _changeDetector.DetectChanges(this)) {
            logd(logId, $"Change: {change}");
            switch (change) {
                case nameof(spawned):
                    _material.color = Color.white;
                    break;
            }
        }
        _material.color = Color.Lerp(_material.color, Color.blue, Time.deltaTime);
    }
}
using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput {
    public Vector3 direction;
    public override string ToString() => "Direction=" + direction;
}

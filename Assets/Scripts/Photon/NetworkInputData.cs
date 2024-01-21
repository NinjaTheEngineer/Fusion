using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput {

    public const byte MOUSEBUTTON0 = 0x01;
    public const byte MOUSEBUTTON1 = 0x02;
    public const byte KEYCODE_SPACE = 0x03;
    public NetworkButtons buttons;
    public Vector3 direction;
    public override string ToString() => "Direction=" + direction;
}

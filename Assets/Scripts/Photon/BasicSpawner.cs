using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using NinjaTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : NinjaMonoBehaviour, INetworkRunnerCallbacks {

    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private bool _mouseButton0;
    private bool _mouseButton1;
    void Update() {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
    }
    async void StartGame(GameMode mode) {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        gameObject.AddComponent<RunnerSimulatePhysics3D>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs() {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
    private void OnGUI() {
        var logId = "OnGUI";
        if (_runner == null) {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host")) {
                logd(logId, "Start Game => Host");
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join")) {
                logd(logId, "Start Game => Client");
                StartGame(GameMode.Client);
            }
        }
    }

    //Callback when NetworkRunner successfully connects to a server or host.
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) {
        var logId = "OnConnectedToServer";
        logd(logId, "Connected to server Runner=" + runner.logf());
    }
    //Callback when NetworkRunner fails to connect to a server or host.
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
        var logId = "OnConnectFailed";
        logw(logId, "Connect failed Runner=" + runner.logf() + " remoteAddress=" + remoteAddress.logf() + " reason=" + reason);
    }
    //Callback when NetworkRunner receives a Connection Request from a Remote Client
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
        var logId = "OnConnectRequest";
        logd(logId, "Connect request Runner=" + runner.logf() + " request=" + request.logf() + " token=" + token.logf());
    }
    //Callback is invoked when the Authentication procedure returns a response from the Authentication Server
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
        var logId = "OnCustomAuthenticationResponse";
        logd(logId, "Custom authentication response Runner=" + runner.logf() + " data=" + data.logf());
    }
    //Callback when NetworkRunner disconnects from a server or host.
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
        var logId = "OnDisconnectedFromServer";
        logd(logId, "Disconnected from server Runner=" + runner.logf() + " reason=" + reason);
    }
    //Callback is invoked when the Host Migration process has started
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
        var logId = "OnHostMigration";
        logd(logId, "Host migration Runner=" + runner.logf() + " hostMigrationToken=" + hostMigrationToken.logf());
    }
    //Callback from NetworkRunner that polls for user inputs. The NetworkInput that is supplied expects: input.Set(new CustomINetworkInput() { /* your values *‍/ });
    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) {
        var logId = "OnInput";

        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        logd(logId, "Input Runner=" + runner.logf() + " Setting Data=" + data.logf() + " to NetworkInput=" + input.logf(), true);

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, _mouseButton1);

        _mouseButton0 = false;
        _mouseButton1 = false;

        input.Set(data);

    }
    //Callback from NetworkRunner when an input is missing.
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
        var logId = "OnInputMissing";
        logw(logId, "Input missing Runner=" + runner.logf() + " player=" + player.logf() + " input=" + input.logf());
    }
    //Callback from a NetworkRunner when a new NetworkObject has entered the Area of Interest
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        var logId = "OnObjectEnterAOI";
        logd(logId, "Object enter AOI Runner=" + runner.logf() + " obj=" + obj.logf() + " player=" + player.logf());
    }
    //Callback from a NetworkRunner when a new NetworkObject has exit the Area of Interest
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        var logId = "OnObjectExitAOI";
        logd(logId, "Object exit AOI Runner=" + runner.logf() + " obj=" + obj.logf() + " player=" + player.logf());
    }
    //Callback from a NetworkRunner when a new player has joined.
    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        var logId = "OnPlayerJoined";
        logd(logId, "Player joined Runner=" + runner.logf() + " player=" + player.logf());
        if (runner.IsServer) {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3(0, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            logd(logId, "Spawned player Runner=" + runner.logf() + " player=" + player.logf() + " networkPlayerObject=" + networkPlayerObject.logf());
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }
    //Callback from a NetworkRunner when a player has disconnected.
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        var logId = "OnPlayerLeft";
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) {
            runner.Despawn(networkObject);
            logd(logId, "Despawned player Runner=" + runner.logf() + " player=" + player.logf() + " networkObject=" + networkObject.logf());
            _spawnedCharacters.Remove(player);
        }
        logd(logId, "Player left Runner=" + runner.logf() + " player=" + player.logf());
    }
    //Callback is invoked when a Reliable Data Stream is being received, reporting its progress
    /*
     * Parameters
     * runner -   NetworkRunner reference
     * player -   Which PlayerRef the stream is being sent from
     * key -	  ReliableKey reference that identifies the data stream
     * progress - Progress of the stream
    */
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {
        var logId = "OnReliableDataProgress";
        logd(logId, "Reliable data progress Runner=" + runner.logf() + " player=" + player.logf() + " key=" + key.logf() + " progress=" + progress);
    }
    //Callback is invoked when a Reliable Data Stream has been received
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {
        var logId = "OnReliableDataReceived";
        logd(logId, "Reliable data received Runner=" + runner.logf() + " player=" + player.logf() + " key=" + key.logf() + " data=" + data.logf());
    }
    //Callback is invoked when a Scene Load has finished
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) {
        var logId = "OnSceneLoadDone";
        logd(logId, "Scene load done Runner=" + runner.logf());
    }
    //Callback is invoked when a Scene Load has started
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) {
        var logId = "OnSceneLoadStart";
        logd(logId, "Scene load start Runner=" + runner.logf());
    }
    //This callback is invoked when a new List of Sessions is received from Photon Cloud
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        var logId = "OnSessionListUpdated";
        logd(logId, "Session list updated Runner=" + runner.logf() + " sessionList=" + sessionList.logf());
    }
    //Called when the runner is shutdown
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
        var logId = "OnShutdown";
        logd(logId, "Shutdown Runner=" + runner.logf() + " shutdownReason=" + shutdownReason);
    }
    //This callback is invoked when a manually dispatched simulation message is received from a remote peer
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
        var logId = "OnUserSimulationMessage";
        logd(logId, "User simulation message Runner=" + runner.logf() + " message=" + message.logf());
    }
}

using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FusionMultiController
{
    public class NetworkManagerFusion : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkManagerFusion Instance { get; private set; }

        [SerializeField] private NetworkPrefabRef _playerPrefab;
        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                // Create a unique position for the player
                Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);

                //Spawn works as an Instantiate function to create player
                NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);


                // Keep track of the player avatars so we can remove it when they disconnect && Adding Active player to dictionary
                _spawnedCharacters.Add(player, networkPlayerObject);
            }
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Find and remove the players avatar
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                //Destroys a netwok object
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }
        }

        // Getting Inputs and updating Network Input Data
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new NetworkInputData();

            if (Input.GetKey(KeyCode.W))
            {
                data.direction += Vector3.forward;
                data.isWalking = true;
            }

            if (Input.GetKey(KeyCode.S))
            {
                data.direction += Vector3.back;
                data.isWalking = true;
            }

            if (Input.GetKey(KeyCode.A))
            {
                data.direction += Vector3.left;
                data.isWalking = true;
            }

            if (Input.GetKey(KeyCode.D))
            {
                data.direction += Vector3.right;
                data.isWalking = true;
            }

            if (Input.GetKey(KeyCode.Space))
                data.isJumping = true;

            if (Input.GetKey(KeyCode.F1))
            {
                data.isDancing = true;
                data.danceAnimIndex = 0;
            }
            
            if (Input.GetKey(KeyCode.F2))
            {
                data.isDancing = true;
                data.danceAnimIndex = 1;
            }

            if (Input.GetKey(KeyCode.F3))
            {
                data.isDancing = true;
                data.danceAnimIndex = 2;
            }

            input.Set(data);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }


        private NetworkRunner _runner;

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
        }

        // To Create Host and Join button when game start
        private void OnGUI()
        {
            if (_runner == null)
            {
                if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
                {
                    StartGame(GameMode.Host);
                }
                if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
                {
                    StartGame(GameMode.Client);
                }
            }
        }

        async void StartGame(GameMode mode)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            // Start or join (depends on gamemode) a session with a specific name
            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }
    }
}

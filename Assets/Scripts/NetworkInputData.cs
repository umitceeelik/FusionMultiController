using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    
    public bool isJumping;

    public bool isDancing;
    public int danceAnimIndex;

}
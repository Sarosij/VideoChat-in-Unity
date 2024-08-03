using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
}

public class PlayerInputHandler : MonoBehaviour
{
    Vector2 inputVector;
    
    void Update()
    {
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputdata = new NetworkInputData();
        networkInputdata.direction = inputVector;

        return networkInputdata;
    }
}

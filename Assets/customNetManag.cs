using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public float spawnOffset = 2f; // Distance between spawned players
    private int playerCount = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Calculate spawn position
        Vector3 spawnPos = Vector3.right * (spawnOffset * playerCount);
        
        // Spawn the player at the calculated position with default rotation
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position + spawnPos, startPos.rotation)
            : Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        
        // Add the player to the network
        base.OnServerAddPlayer(conn);
        
        playerCount++;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        playerCount--;
    }

    public override void OnStopServer()
    {
        playerCount = 0;
        base.OnStopServer();
    }
}

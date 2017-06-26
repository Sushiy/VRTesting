using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class MP_Health : NetworkBehaviour
{
    public static readonly int MAX_HEALTH = 100;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = MAX_HEALTH;

    public RectTransform old_Healthbar;

    private NetworkStartPosition[] spawnPoints;

    [SerializeField]
    private Healthbar m_healthbar;

    private ParticleSystem m_winParticles;

    private void Awake()
    {
        m_winParticles = transform.Find("WinParticles").GetComponent<ParticleSystem>();
        Assert.IsNotNull(m_winParticles);
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            // play win particles for some seconds
            m_winParticles.Play();

            currentHealth = MAX_HEALTH;

            // actually detsroys and respawns them
            //if (destroyOnDeath)
            //{
            //    Destroy(gameObject);
            //}
            //else
            //{
            //    currentHealth = MAX_HEALTH;

            //    // called on the Server, invoked on the Clients
            //    CmdRespawnSvr();
            //}
        }
    }

    void OnChangeHealth(int currentHealth)
    {
        this.currentHealth = currentHealth;
        //old_Healthbar.sizeDelta = new Vector2(currentHealth, old_Healthbar.sizeDelta.y);
        //m_healthbar.UpdateHealth(currentHealth);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            // Set the spawn point to origin as a default value
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array and the array is not empty, pick one at random
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            // Set the player’s position to the chosen spawn point
            transform.position = spawnPoint;
        }
    }

    [Command]
    void CmdRespawnSvr()
    {
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        GameObject newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
        NetworkServer.Destroy(this.gameObject);
        NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer, this.playerControllerId);

    }
}

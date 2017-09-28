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

    public ParticleSystem m_psPlayerDeadExplosion;

    [SerializeField]
    private Healthbar m_healthbar;

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

            //currentHealth = MAX_HEALTH;
            StartCoroutine(DelayExit(1.5f));
        }
    }

    void OnChangeHealth(int currentHealth)
    {
        this.currentHealth = currentHealth;
        //old_Healthbar.sizeDelta = new Vector2(currentHealth, old_Healthbar.sizeDelta.y);
        //m_healthbar.UpdateHealth(currentHealth);
    }

    IEnumerator DelayExit(float seconds)
    {
        //yield return new WaitForSeconds(0.25f);

        m_psPlayerDeadExplosion.Play();
        yield return new WaitForSeconds(seconds);
        CmdRespawnSvr();
    }

    [Command]
    void CmdRespawnSvr()
    {
        NetworkManager manager = NetworkManager.singleton;

        manager.StopHost();

        /*
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        GameObject newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
        NetworkServer.Destroy(this.gameObject);
        NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer, this.playerControllerId);
        */
    }
}

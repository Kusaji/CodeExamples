using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{

    [SyncVar(hook = nameof(UpdateHealthUI))]
    public float currentHealth;
    [SyncVar]
    public float maxHealth;
    [SyncVar]
    public bool isAlive;
    public bool localPlayer;

    public bool isRespawning;

    [SyncVar]
    [SerializeField] string localConnection;
    [SerializeField] private GameObject viewModel;
    [SerializeField] private Text healthUI;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerRespawner respawn;
    [SerializeField] PlayerScore playerScore;
    [SerializeField] DeathScreen deathScreen;



    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isLocalPlayer)
        {
            localPlayer = true;
            isAlive = true;
            healthUI = GameObject.Find("PlayerHealthText").GetComponent<Text>();
            viewModel = GameObject.Find("ViewModel");
            respawn = GameObject.Find("PlayerRespawner").GetComponent<PlayerRespawner>();
            deathScreen = GameObject.Find("BlackScreen").GetComponent<DeathScreen>();
            InitPlayerHealth();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (!isRespawning && !isAlive)
            {
                isRespawning = true;
                StartCoroutine(RespawnPlayerRoutine());
            }
        }
    }

    void UpdateHealthUI(float oldHealth, float newHealth)
    {
        if (isLocalPlayer)
        {
            healthUI.text = $"Health: {newHealth} | {maxHealth}";
            SetPlayerHealth(newHealth);
        }
    }

    [Command]
    public void InitPlayerHealth()
    {
        currentHealth = maxHealth;
        isAlive = true;
    }

    [Command]
    public void SetPlayerHealth(float health)
    {
        if (isAlive)
        {
            currentHealth = health;

            if (currentHealth <= 0)
            {
                isAlive = false;
                
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void AddHealth(float healAmount)
    {
        if (currentHealth + healAmount > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += healAmount;
        }
    }

    public void RespawnPlayer()
    {
        GetComponent<MeshRenderer>().enabled = false;
        InitPlayerHealth();
        gameObject.transform.position = respawn.GetRandomRespawnPoint().transform.position;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public IEnumerator RespawnPlayerRoutine()
    {
        Debug.Log($"Respawn Player CoRoutine called on {gameObject.name}");
        deathScreen.deathOverlay.SetActive(true);
        gameObject.transform.position = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        gameObject.transform.position = respawn.GetRandomRespawnPoint().transform.position;
        InitPlayerHealth();
        isAlive = true;
        isRespawning = false;
        deathScreen.deathOverlay.SetActive(false);
        Debug.Log($"Respawn Player CoRoutine ended on {gameObject.name}");
    }

    [Command]
    public void HideMesh(GameObject player, bool isHidden)
    {
        respawn.HideMesh(player, isHidden);
    }

    
    public void TakeDamage(float damage)
    { 
        if (isAlive)
        {
            currentHealth -= damage;
            

            if (currentHealth <= 0)
            {
                isAlive = false;
                playerScore.AddDeath();
            }
        }
    }

    //Used for falling off map, etc..
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("KillTrigger"))
        {
            TakeDamage(currentHealth);
        }
    }
}

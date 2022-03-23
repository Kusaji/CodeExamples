using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField] GameObject cameraObject;
    [SerializeField] GameObject weaponObject;
    [SerializeField] GameObject viewModelMuzzlePoint;
    [SerializeField] Collider myCollider;
    [SerializeField] PlayerAudio speaker;
    [SerializeField] PlayerScore playerScore;

    [SyncVar]
    public bool isCarryingItem;
    [SyncVar]
    public bool isShooting;
    public bool shootDelay;
    [SyncVar]
    public GameObject carriedItem;
    [SyncVar]
    public GameObject itemCopy;

    [SyncVar]
    public GameObject activeWeaponMuzzle;


    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ServerPrefabSpawner bulletSpawner;
    
    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            base.OnStartLocalPlayer();
            cameraObject = GameObject.Find("Camera");
            isCarryingItem = false;
            weaponObject.SetActive(false);
            viewModelMuzzlePoint = GameObject.Find("ViewModelMuzzlePoint");
            gameObject.layer = 2;
            isShooting = false;
            bulletSpawner = GameObject.Find("ServerPrefabSpawner").GetComponent<ServerPrefabSpawner>();
        }
    }

/*    void Start()
    {
        if (isLocalPlayer)
        {
            weaponObject.SetActive(false);
            cameraObject = Camera.main.gameObject;
            viewModelMuzzlePoint = GameObject.Find("ViewModelMuzzlePoint");
            gameObject.layer = 2;
            speaker = GameObject.Find("AudioController").GetComponent<PlayerAudio>();
            isShooting = false;
        }
    }
*/
    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0) && !shootDelay)
            {
                isShooting = true;
                StartCoroutine(AutomaticFire());
            }

            if (Input.GetMouseButtonUp(0))
            {
                isShooting = false;
            }
        }
    }

    IEnumerator AutomaticFire()
    {
        while (isShooting)
        {
            shootDelay = true;
            Shoot(10);
            yield return new WaitForSeconds(0.15f);
            shootDelay = false;
        }
    }


    public void Shoot(float damage)
    {
        
        RaycastHit hit;
        speaker.CmdPlaySound(0);
        CmdCreateBullet(viewModelMuzzlePoint.transform.eulerAngles);

        if (Physics.Raycast(cameraObject.transform.position, cameraObject.transform.TransformDirection(Vector3.forward), out hit, 500f))
        {
            var hitObject = hit.collider.gameObject;
            myCollider.enabled = true;

            if (hitObject != gameObject && hit.collider.gameObject.CompareTag("Player"))
            {      
                //red line if we hit a player
                Debug.DrawLine(viewModelMuzzlePoint.transform.position, hit.point, Color.red, 1f);
                SendDamageCommand(hitObject, damage);

            }
            else if (hitObject != gameObject)
            {
                //Yellow line if we hit a game object
                Debug.DrawLine(viewModelMuzzlePoint.transform.position, hit.point, Color.yellow, 1f);

            }
        }
        else
        {
            Debug.DrawRay(viewModelMuzzlePoint.transform.position, cameraObject.transform.TransformDirection(Vector3.forward * 50f), Color.yellow, 1f);
        }
    }

    [Command]
    void CmdCreateBullet(Vector3 rotation)
    {
        var bullet = Instantiate(bulletPrefab, activeWeaponMuzzle.transform.position - new Vector3(0, -0.25f, 0), Quaternion.Euler(rotation));
        NetworkServer.Spawn(bullet);
    }

    [Command(requiresAuthority = false)]
    void SendDamageCommand(GameObject hitObject, float damage)
    {
        hitObject.GetComponent<PlayerHealth>().TakeDamage(damage);

        if (hitObject.GetComponent<PlayerHealth>().currentHealth <= 0)
        {
            playerScore.AddKill();
        }
    }
}

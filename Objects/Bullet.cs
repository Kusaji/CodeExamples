using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * 300 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        NetworkServer.Destroy(gameObject);
        Destroy(gameObject);
    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(5);
        NetworkServer.Destroy(gameObject);
    }
}

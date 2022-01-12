using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{

    [SerializeField] float moveSpeed = 800f;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(transform.position.z>EnemyManager.Instance.enemySpawnDistance)
        {
            Destroy(gameObject);
        }
        rb.velocity = new Vector3(0f, 0f, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyLogic>().DestroyEnemy();
            Destroy(gameObject);
        }
    }
}

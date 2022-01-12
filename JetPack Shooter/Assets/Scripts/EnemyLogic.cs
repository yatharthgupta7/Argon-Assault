using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] float moveSpeed = 20f;
    Rigidbody rb;
    float removePositionZ;
    [SerializeField]Material[] baseMat;
    [SerializeField] Material targetMat;
    [SerializeField]Renderer[] renderers;
    [SerializeField] ParticleSystem  particle;
    [SerializeField] AudioClip destroy;
    [SerializeField]AudioSource audio;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        removePositionZ = Camera.main.transform.position.z;
        //audio = GetComponent<AudioSource>();
        //renderers = GetComponentsInChildren<Renderer>();
    }

    public void ResetRenderer()
    {
        if(renderers==null)
        {
            return;
        }

        for(int x=0;x<renderers.Length;x++)
        {
            renderers[x].material = baseMat[x];
        }
    }

    public void LevelUp()
    {
        moveSpeed = moveSpeed * 2.0f;
    }

    public void SetTargetMaterial()
    {
        
        if(renderers==null)
        {
            return;
        }

        foreach(Renderer rand in renderers)
        {
            rand.material = targetMat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.z<removePositionZ)
        {
            DestroyEnemy1();
            EnemyManager.Instance.aliveEnemy.Remove(gameObject);
        }
        Vector3 movemntVector = new Vector3(0f, 0f,- moveSpeed * Time.deltaTime);
        rb.velocity = movemntVector;
    }
    public void DestroyEnemy1()
    {
        EnemyManager.Instance.aliveEnemy.Remove(gameObject);
        Destroy(gameObject);
    }

    private void PlayDestroySound()
    {
        audio.Play();
    }

    public void DestroyEnemy()
    {
        EnemyManager.Instance.aliveEnemy.Remove(gameObject);
        //play paricle effect
        //audio.PlayOneShot(destroy);
        ParticleSystem ob=Instantiate(particle, gameObject.transform.position, Quaternion.identity);
        PlayDestroySound();
        //disable movement

        //disable collider

        //destrop game object
        Destroy(gameObject);
        //StartCoroutine(explosion());
        if(ob.IsAlive())
        {
            Destroy(ob);
        }
    }
    IEnumerator explosion()
    {
        yield return new WaitForSeconds(0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().OnEnemyImpact();
            DestroyEnemy();
        }
    }
}

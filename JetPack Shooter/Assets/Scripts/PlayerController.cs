using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float maxRotation = 25f;
    [SerializeField] GameManager gameManager;
    [SerializeField]const int MAX_HEALTH = 5;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip death;
    [SerializeField] ParticleSystem particle;
    [SerializeField] GameObject bullet;
    float timer = 0.0f;
    Vector3 tilt = Vector3.zero;//variable for accelerometer
    Vector3 calibratedtilt = Vector3.zero;
    int currentHealth;
    public AudioSource audioSource;
    public Transform[] bulletSpawnPoints;
    public float fireInterval = 0.1f;
    bool canFire = true;

    float minX, maxX, minY, maxY;
    Rigidbody rb;

    Vector3 raycastDirection = new Vector3(0f, 0f, 1f);
    public float raycastDst = 100f;
    int layerMask;
    private List<GameObject> previousTarget = new List<GameObject>();
    public bool canMove = false;

    private void Awake()
    {
        Calibrate();
    }
    void Start()
    {
        Calibrate();
        rb = GetComponent<Rigidbody>();
        currentHealth = MAX_HEALTH;
        SetUpBoundaries();
        layerMask = LayerMask.GetMask("EnemyRaycastLayer");
        audioSource = GetComponent<AudioSource>();
    }

    public void FireBullets()
    {
        if (canFire)
        {
            foreach(Transform t in bulletSpawnPoints)
            {
                Instantiate(bullet, t.position, bullet.transform.rotation);
                if(gameManager.audioOn)
                {
                    audioSource.PlayOneShot(shoot);
                }
            }

            canFire = false;

            StartCoroutine(ReloadDelay());
        }
    }

    IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(fireInterval);
        canFire = true;
    }

    private void RotatePlayer()
    {
        float currentX = transform.position.x;
        float newRoationZ;

        if (currentX < 0)
        {
            newRoationZ = Mathf.Lerp(0f, -maxRotation, currentX / minX);
        }
        else
        {
            newRoationZ = Mathf.Lerp(0f, maxRotation, currentX / maxX);
        }

        Vector3 currentRoationVector3 = new Vector3(0f, 0f, newRoationZ);
        Quaternion newRotation = Quaternion.Euler(currentRoationVector3);
        transform.localRotation = newRotation;
    }

    void CalculateBoundaries()
    {
        Vector3 currentPos = transform.position;

        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.y = Mathf.Clamp(currentPos.y, minY, 6.0f);
        transform.position = currentPos;
    }

    void SetUpBoundaries()
    {
        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorners = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, camDistance));
        Vector2 topCorners = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, camDistance));

        Bounds gameObjectbounds = GetComponent<Collider>().bounds;
        float objectWidth = gameObjectbounds.size.x;
        float objectHeight = gameObjectbounds.size.y;
        minX = bottomCorners.x+objectWidth;
        maxX = topCorners.x-objectWidth;
        minY = bottomCorners.y+ objectHeight;
        maxY = topCorners.y- objectHeight;

        EnemyManager.Instance.maxX = maxX;
        EnemyManager.Instance.minX = minX;

        EnemyManager.Instance.minY = minY;
        EnemyManager.Instance.maxY = maxY;
    }

    void Update()
    {
        RotatePlayer();
        CalculateBoundaries();
        RaycastForEnemy();
        timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if(!canMove)
        {
            Calibrate();
        }
        if (!canMove)
            return;
        tilt.x = Input.acceleration.x - calibratedtilt.x;
        tilt.y = -(Input.acceleration.y - calibratedtilt.y);
        //tilt.z = -Input.acceleration.z - calibratedtilt.z;
        //tilt = Quaternion.Euler(90, 0, 0) * tilt;
        //Vector3 movementVector = new Vector3(horizontal, -vertical, 0);
        rb.velocity = tilt * moveSpeed*Time.deltaTime;
    }

    public void OnEnemyImpact()
    {
        currentHealth--;
        gameManager.ChangeHealthBar(MAX_HEALTH, currentHealth);

        if(currentHealth==0)
        {
            OnPlayerDeath();
        }
    }

    private void RaycastForEnemy()
    {
        List<GameObject> currentTargets = new List<GameObject>();
        foreach(Transform bulletSpawnPoints in bulletSpawnPoints)
        {
            RaycastHit hit;
            Ray ray = new Ray(bulletSpawnPoints.position, raycastDirection);
            if(Physics.Raycast(ray, out hit,raycastDst,layerMask))
            {
                GameObject target = hit.transform.gameObject;
                currentTargets.Add(target);
            }
        }
        bool listIsChanged = false;
        if(currentTargets.Count!=previousTarget.Count)
        {
            listIsChanged = true;
        }
        else
        {
            for(int x=0;x<currentTargets.Count;++x)
            {
                if(currentTargets[x]!=previousTarget[x])
                {
                    listIsChanged = true;
                }
            }
        }

        if(listIsChanged)
        {
            EnemyManager.Instance.UpdateEnemy(currentTargets);
            previousTarget = currentTargets;
        }
    }
    public void Calibrate()
    {
        //Gets devices physical rotation in 3D space
        calibratedtilt.x = Input.acceleration.x;
        calibratedtilt.y = Input.acceleration.y;
        calibratedtilt.z = Input.acceleration.z;
    }

    IEnumerator explosion()
    {
        yield return new WaitForSeconds(1f);
    }
    private void OnPlayerDeath()
    {
        //play animation
        Instantiate(particle, gameObject.transform.position, Quaternion.identity);
        if(gameManager.audioOn)
        {
            audioSource.PlayOneShot(death);
        }
        StartCoroutine(explosion());
        gameManager.Death();
    }
}

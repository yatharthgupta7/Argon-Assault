using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    [SerializeField] Transform[] lookAtAr;
    [SerializeField] PlayerController playerController;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] GameObject instructions;
    [SerializeField] AdsManager ads;

    bool bannerHidden = false;
    Transform lookAt;
    Vector3 startOffset;
    Vector3 moveVector;

    float transition = 0.0f;
    float animationDuration = 7.0f;
    Vector3 animationOPffset = new Vector3(0, 5, 10);
    int index;
    void Start()
    {
        //will come into use when implemented plane selectiom
        //index = PlayerPrefs.GetInt("PlaneSelected");
        lookAt = GameObject.FindGameObjectWithTag("Player").transform;
        lookAt = lookAtAr[index].transform;
        startOffset = transform.position - lookAt.position;
        ads.ShowBanner();
    }

    // Update is called once per frame
    void Update()
    {
        moveVector = lookAt.position + startOffset;

        moveVector.x = 0;
        moveVector.y = Mathf.Clamp(moveVector.y, 3, 5);

        if (transition > 1.0f)
        {
            instructions.SetActive(false);
            if(bannerHidden == false)
            {
                ads.HideBanner();
                bannerHidden = true;
            }
            transform.position = moveVector;
            playerController.canMove = true;
            enemyManager.spawn = true;
            //instructions.SetActive(false);
        }
        else
        {
            transform.position = Vector3.Lerp(moveVector + animationOPffset, moveVector, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            transform.LookAt(lookAt.position + Vector3.up);
        }
    }
}

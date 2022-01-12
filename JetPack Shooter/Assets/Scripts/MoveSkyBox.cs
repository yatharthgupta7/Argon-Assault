using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSkyBox : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 2.0f;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.deltaTime * rotateSpeed);
    }
}

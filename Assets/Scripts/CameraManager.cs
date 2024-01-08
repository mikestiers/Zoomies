using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject player;
    public float dampening;
    public float help;
    public Vector3 offset;
    private PlayerControllerWheelColliders playerController;

    //void Update()
    //{
    //    transform.position = Vector3.Lerp(transform.position, player.transform.position + player.transform.TransformVector(offset), help * (dampening * Time.deltaTime));
    //    transform.LookAt(player.transform);
    //}

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + player.transform.TransformVector(offset), dampening * Time.fixedDeltaTime);
        transform.LookAt(player.transform);
    }

    private void Awake()
    {
        playerController = player.GetComponent<PlayerControllerWheelColliders>();
    }
}

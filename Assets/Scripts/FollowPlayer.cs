using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerTransform;

    void FixedUpdate()
    {
        this.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, this.transform.position.z);
    }
}

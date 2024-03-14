using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 dir;
    [SerializeField]
    private float speed = 2;
    private float gravity = -9.8f;
    private void move()
    {
        dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (characterController.isGrounded == false)
            dir += new Vector3(0, gravity, 0);
        characterController.Move(dir * Time.deltaTime * speed);
    }
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveValue;
    public float speed;
    public Rigidbody rigidBody;

    public int numPickups = 4;

    private int count;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;

    private void Start()
    {
        count = 0;
        winText.text = "";
        SetCountText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    private void SetCountText()
    {
        scoreText.text = $"Score: {count}";
        if(count >= numPickups)
        {
            winText.text = "You Win!";
        }
    }

    private void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        rigidBody.AddForce(movement * speed * Time.fixedDeltaTime);
    }
}

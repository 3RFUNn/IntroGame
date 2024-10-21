using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputActionAsset inputActions;
    public InputAction moveAction; // Variable for Move action
    public InputAction fireAction; // New variable for Fire action

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        moveAction = inputActions.FindAction("Move"); // Assign Move action
        fireAction = inputActions.FindAction("Fire"); // Assign Fire action
    }

    // Update is called once per frame
    void Update()
    {

    }
}
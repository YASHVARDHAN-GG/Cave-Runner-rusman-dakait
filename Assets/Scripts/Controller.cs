using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour

{
   public PlayerControl playerInput;
    public Vector2 movement;
    public bool jumpInput;
    public bool dash;

    public void Awake() => playerInput = new PlayerControl();
    public void OnEnable() => playerInput.Enable();

    public void OnDisable() => playerInput.Disable();
    private void Update()
    {
        playerInput.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        playerInput.Player.Move.canceled += ctx => movement = Vector2.zero;
        playerInput.Player.Jump.performed += ctx =>jumpInput = true;
        playerInput.Player.Jump.canceled += ctx => jumpInput = false;
        playerInput.Player.Dash.performed += ctx => dash = true;
        playerInput.Player.Dash.canceled += ctx => dash = false;

    }


}

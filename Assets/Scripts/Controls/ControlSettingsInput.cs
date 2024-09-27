using UnityEngine;

public class ControlSettingsInput : MonoBehaviour, IInput
{
    [SerializeField] private ControlSettings controls;

    public void SetControls(ControlSettings controls)
    {
        this.controls = controls;
    }

    public Vector2 GetAxisInput()
    {
        if (controls == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;
        if (Input.GetKey(controls.Up)) input += Vector2.up;
        if (Input.GetKey(controls.Down)) input += Vector2.down;
        if (Input.GetKey(controls.Left)) input += Vector2.left;
        if (Input.GetKey(controls.Right)) input += Vector2.right;
        return input;
    }

    public bool GetPrimaryAction()
    {
        if (controls == null)
            return false;

        return Input.GetKeyDown(controls.PrimaryAction);
    }

    public bool GetSecondaryAction()
    {
        if (controls == null)
            return false;

        return Input.GetKeyDown(controls.SecondaryAction);
    }
}

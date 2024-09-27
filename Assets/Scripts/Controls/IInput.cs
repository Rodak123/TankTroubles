using UnityEngine;

public interface IInput
{
    public Vector2 GetAxisInput();

    public bool GetPrimaryAction();
    public bool GetSecondaryAction();
}

using UnityEngine;

public class ExplosionEvents : MonoBehaviour
{
    public void ExplosionFinished()
    {
        Destroy(gameObject);
    }

}

using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance { get; private set; }

    [SerializeField] private GameObject explosionPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"{typeof(ExplosionManager)} instance exists already, aborting", this);
            return;
        }
        Instance = this;
    }

    public void CreateExplosionAt(Vector3 position) => Instantiate(explosionPrefab, position, Quaternion.identity, transform);

}

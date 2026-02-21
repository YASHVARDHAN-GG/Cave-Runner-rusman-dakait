using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value;
    private bool hasTrigger;

    private CoinManager coinManager;

    private void Start()
    {
        coinManager = CoinManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !hasTrigger)
        {
            hasTrigger =true;
            coinManager.ChangeCoins(value);
            Destroy(gameObject);
        }
    }
}

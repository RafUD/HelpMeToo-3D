using UnityEngine;

public class CoinMove : MonoBehaviour
{
    Coin coinScript;

    void Start()
    {
        coinScript = GetComponent<Coin>();
    }

    void Update()
    {
        if (coinScript.playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                coinScript.playerTransform.position,
                coinScript.moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Use the unified collection system
            CollectCoin collectScript = GetComponent<CollectCoin>();
            if (collectScript != null)
            {
                collectScript.CollectThisCoin();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
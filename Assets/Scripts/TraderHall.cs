using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideMonoScript]
public class TraderHall : MonoBehaviour
{
    [Title("Trader Hall", "Responsible for spawning in the nessesary traders for trading")]
    public int TraderCount = 1;
    public StockMarket Market;
    public GameObject TraderPrefab;

    void Start()
    {
        if (Market == null)
            return;

        if (!Market.IsOpen)
        {
            Market.OnOpened += SpawnTraders;
        }
        else
        {
            SpawnTraders();
        }
    }

    private void SpawnTraders()
    {
        Market.OnOpened -= SpawnTraders;

        for (int i = 0; i < TraderCount; i++)
        {
            SpawnTrader();
        }
    }

    [Button]
    private void SpawnTrader()
    {
        Object.Instantiate(TraderPrefab);
    }
}

using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Globalization;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class TraderAgent : Agent
{
    public static int Count = 0;

    private int m_StartTime = 0;
    private int m_Id =-1;
    private TradingAccount m_Account;
    private Queue<decimal> m_PreviousPrices;
    public decimal InitialInvestment = 100000;
    private readonly int m_MaxQueuePrices = 10;

#if UNITY_EDITOR
    [Sirenix.OdinInspector.Button, Sirenix.OdinInspector.HideInEditorMode]
    private static void OpenInspector()
    {
        Sirenix.OdinInspector.Editor.OdinEditorWindow.GetWindow<TraderInspector>().Show();
    }
#endif

    public void Start()
    {
        m_Id = Count;
        Count++;
        m_Account = new TradingAccount(InitialInvestment);
        m_PreviousPrices = new Queue<decimal>();
    }

    private void OnDestroy()
    {
        m_Id = -1;
        Count--;
    }

    void OnGUI()
    {
        int xOffset = (m_Id % 6) * 200;
        int yOffset = (m_Id / 6) * 130;
        int time = (int)Time.timeSinceLevelLoad;

        GUI.Label(new Rect(10 + xOffset, 10 + yOffset, 200, 20), "Pouch: " + m_Account?.CurrencyPouch.ToString("C", CultureInfo.CurrentCulture));
        GUI.Label(new Rect(10 + xOffset, 30 + yOffset, 200, 20), "Assets: " + m_Account?.AssetPouch.ToString("F7", CultureInfo.InvariantCulture));
        GUI.Label(new Rect(10 + xOffset, 50 + yOffset, 200, 20), "Total: " + m_Account?.GetTotalWealth().ToString("C", CultureInfo.CurrentCulture));
        GUI.Label(new Rect(10 + xOffset, 70 + yOffset, 200, 20), "Reward: " + GetCumulativeReward());
        GUI.Label(new Rect(10 + xOffset, 90 + yOffset, 200, 20), "Elapsed: " + (time - m_StartTime));
    }

    public decimal GetTotalWealth()
    {
        return m_Account != null ? m_Account.GetTotalWealth() : 0m;
    }

    public decimal GetCurrentStanding()
    {
        return m_Account != null ? m_Account.GetStanding() : 0m;
    }

    public decimal GetCurrency()
    {
        return m_Account != null ? m_Account.CurrencyPouch : 0m;
    }

    public decimal GetAssets()
    {
        return m_Account != null ? m_Account.AssetPouch : 0m;
    }

    public override void OnEpisodeBegin()
    {
        // Reset all variables and grab new set of data
        Debug.Log($"Training started");
        m_Account = new TradingAccount(InitialInvestment);
        m_PreviousPrices = new Queue<decimal>();
        m_StartTime = (int)Time.timeSinceLevelLoad;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var currentPrice = StockMarket.CurrentPrice;
        m_PreviousPrices.Enqueue(currentPrice);
        if (m_PreviousPrices.Count > m_MaxQueuePrices)
        {
            m_PreviousPrices.Dequeue();
        }

        sensor.AddObservation((float)(currentPrice / TradingAccount.MaxCurrency));
        sensor.AddObservation((float)((currentPrice * (1 + StockMarket.ExchangeCostDecimal)) / TradingAccount.MaxCurrency));
        sensor.AddObservation((float)(m_Account.RealizedCurrency / TradingAccount.MaxCurrency));
        sensor.AddObservation((float)(m_Account.CurrencyPouch / TradingAccount.MaxCurrency));
        sensor.AddObservation((float)(m_Account.AssetPouch / TradingAccount.MaxAssets));
        sensor.AddObservation((float)(m_Account.GetTotalWealth() / TradingAccount.MaxCurrency));

        int added = 0;
        foreach (var price in m_PreviousPrices)
        {
            added++;
            sensor.AddObservation((float)(price / TradingAccount.MaxCurrency));
        }

        // Padding data if not enough is provided
        for (int i = 0; i < (m_MaxQueuePrices - added); i++)
        {
            sensor.AddObservation((float)(currentPrice / TradingAccount.MaxCurrency));
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions[0] = Input.GetKey(KeyCode.UpArrow) ? 0 : Input.GetKey(KeyCode.DownArrow) ? 20 : 10;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        decimal value = actionBuffers.DiscreteActions[0] - 10;
        value /= 10;
        if (value > 0)
        {
            m_Account.Buy(value);
        }
        else if (value < 0)
        {
            m_Account.Sell(value);
        }

        // Directly corralate wealth with reward. The more wealth, the more points!
        float wealthPoints = m_Account.GetWealthPoints();
        float current = GetCumulativeReward();
        SetReward(wealthPoints - current);
        if (m_Account.Unhealthy())
        {
            Debug.Log($"Training failed! Ending Episode...");
            AddReward(-100f);
            EndEpisode();
        }
        else if(m_Account.GeneratedRequirements())
        {
            Debug.Log($"Training succeeded! Ending Episode...");
            AddReward(100f);
            EndEpisode();
        }
    }
}

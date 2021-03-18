using System.Collections.Generic;

public class TradingAccount
{
    private float m_MilestonePoints = 0;
    private readonly Queue<Milestone> m_Milestones;
    private readonly decimal m_InitialInvestment = 0;
    public decimal RealizedCurrency { get; private set; }
    public decimal CurrencyPouch { get; private set; }
    public decimal AssetPouch { get; private set; }

    public static decimal MaxCurrency => 1000000;
    public static decimal MaxAssets => 100;

    public TradingAccount(decimal initialInvestment)
    {
        m_InitialInvestment = initialInvestment;
        CurrencyPouch = initialInvestment;
        RealizedCurrency = CurrencyPouch;
        AssetPouch = 0;
        m_MilestonePoints = 0;
        m_Milestones = new Queue<Milestone>();
        m_Milestones.Enqueue(new Milestone(0.01m));
        m_Milestones.Enqueue(new Milestone(0.02m));
        m_Milestones.Enqueue(new Milestone(0.03m));
        m_Milestones.Enqueue(new Milestone(0.04m));
        m_Milestones.Enqueue(new Milestone(0.05m));
        m_Milestones.Enqueue(new Milestone(0.1m));
        m_Milestones.Enqueue(new Milestone(0.2m));
        m_Milestones.Enqueue(new Milestone(0.3m));
        m_Milestones.Enqueue(new Milestone(0.4m));
        m_Milestones.Enqueue(new Milestone(0.5m));
        Buy(1);
    }

    /// <summary>
    /// Buys a given decimal amount of assets. EX: If the account has 100c & 1 is passed, then the account will buy 100c worth of assets.
    /// </summary>
    /// <param name="value">The value decimal to buy [0,1]</param>
    public bool Buy(decimal value)
    {
        value = System.Math.Abs(value);
        decimal assetPrice = StockMarket.CurrentPrice * (1 + StockMarket.ExchangeCostDecimal);
        if (assetPrice == 0)
            return false;

        // Calculate cost
        decimal budget = CurrencyPouch * value;
        decimal assetCount = budget / assetPrice;

        // Make transaction
        CurrencyPouch -= budget;
        AssetPouch += assetCount;
        return true;
    }

    /// <summary>
    /// Sells a given decimal amount of assets. EX: If the account has 100 assets & 1 is passed, then the account will sell 100 assets.
    /// </summary>
    /// <param name="value">The value decimal to sell [0,1]</param>
    public bool Sell(decimal value)
    {
        value = System.Math.Abs(value);
        decimal assetPrice = StockMarket.CurrentPrice;
        if (assetPrice == 0)
            return false;

        // Calculate cost
        decimal liquidity = AssetPouch * value;
        decimal currencyValue = liquidity * assetPrice;

        // Make transaction
        AssetPouch -= liquidity;
        CurrencyPouch += currencyValue;
        return true;
    }

    public bool Unhealthy()
    {
        // If we've lost a percentage of our initial investment
        decimal currentStanding = GetRealizedStanding();
        return currentStanding <= -0.01m;
    }

    public bool GeneratedRequirements()
    {
        // If the wealth has doubled
        decimal currentStanding = GetStanding();
        return currentStanding >= 1m;
    }

    public float GetWealthPoints()
    {
        // Percentage value as points
        decimal standing = GetRealizedStanding();
        if (standing == 0)
            return 0;

        if (m_Milestones.Count > 0)
        {
            while (m_Milestones.Peek().HasAchieved(standing))
            {
                m_MilestonePoints += m_Milestones.Dequeue().Points;
            }
        }

        decimal direction = standing / System.Math.Abs(standing);
        decimal val = 100m * standing;
        return ((float)(direction * val * val)) + m_MilestonePoints;
    }

    public decimal GetRealizedStanding()
    {
        return (GetTotalWealth() / RealizedCurrency) - 1;
    }

    public decimal GetStanding()
    {
        return (GetTotalWealth() / m_InitialInvestment) - 1;
    }

    public decimal GetTotalWealth()
    {
        var wealth = CurrencyPouch + (AssetPouch * StockMarket.CurrentPrice);
        if (wealth > RealizedCurrency)
        {
            RealizedCurrency = wealth;
            return 0;
        }

        return wealth;
    }
}

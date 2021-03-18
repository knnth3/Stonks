#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using System.Globalization;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

[HideMonoScript]
public class TraderInspector : OdinEditorWindow
{
    [Title("Trader visible properties")]
    public TraderAgent Trader;
    [ReadOnly]
    public string Currency = "0";
    [ReadOnly]
    public string Assets = "0";
    [ReadOnly]
    public string TotalWealth = "$0.00";
    [ReadOnly]
    public string Reward = "0";
    [ReadOnly]
    public string Standing = "0";

    public decimal sStanding => (Trader != null) ? Trader.GetCurrentStanding() : 0;
    public decimal sCurrency => (Trader != null) ? Trader.GetCurrency() : 0;
    public decimal sAssets => (Trader != null) ? Trader.GetAssets() : 0;
    public decimal sTotalWealth => (Trader != null) ? Trader.GetTotalWealth() : 0;
    public float sReward => (Trader != null) ? Trader.GetCumulativeReward() : 0;

    [MenuItem("Stonks/Trader Inspector")]
    private static void OpenWindow()
    {
        GetWindow<TraderInspector>().Show();
    }

    private void Update()
    {
        TotalWealth = sTotalWealth.ToString("C", CultureInfo.CurrentCulture);
        Reward = "" + sReward;
        Currency = "" + sCurrency;
        Assets = "" + sAssets;
        Standing = "" + sStanding;
        Repaint();
    }
}

#endif
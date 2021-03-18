using UnityEngine;
using PriceViewers.Viewers;
using Sirenix.OdinInspector;
using System.Globalization;

[HideMonoScript]
public class StockMarket : MonoBehaviour
{
    // mlagents-learn --force
    // or
    // mlagents-learn --run-id=test2
    private bool m_bPaused = false;
    private static StockMarket m_Instance;
    private CryptoViewer m_CryptoViewer;
    [Title("Stock Market", "Manages the retrieval of current crypto prices")]
    public bool Headless = true;
    public string CryptoCode = "BTC";
    public event System.Action OnOpened;

    public static StockMarket Instance { get { return m_Instance; } }

    public bool IsOpen { get; private set; }
    public static decimal CurrentPrice => (Instance != null && Instance.m_CryptoViewer != null) ? (decimal)Instance.m_CryptoViewer.CurrentPrice : 10000m;
    public static decimal ExchangeCostDecimal => 0.001m;
    public static string Code => (Instance != null) ? Instance.CryptoCode : "";

    private void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_Instance = this;
        }
    }

    void Start()
    {
        Initialize();
    }

    public void OnDestroy()
    {
        Debug.Log("Stock market is closing!");
        IsOpen = false;
        m_CryptoViewer?.StopService();
        m_CryptoViewer = null;
    }

    void OnGUI()
    {
        var position = transform.localPosition;
        GUI.Label(new Rect(position.x, position.y, 100, 20), CryptoCode + ": " + CurrentPrice.ToString("C", CultureInfo.CurrentCulture));
    }

    private void FixedUpdate()
    {
        if (m_CryptoViewer.RefreshingPrices && !m_bPaused)
        {
            PauseStockMarket();
        } else if (m_bPaused)
        {
            ResumeStockMarket();
        }
    }

    [Button, DisableInEditorMode]
    private async void Initialize()
    {
        Debug.Log("Attempting to contact the stock market api...");
        m_CryptoViewer = new CryptoViewer();
        if (await m_CryptoViewer.Initialize(CryptoCode, Headless))
        {
            Debug.Log("Stock market is opening!");
            m_CryptoViewer.StartService();
            IsOpen = true;
            OnOpened?.Invoke();
        } else
        {
            Debug.Log("Failed to contact the stock market api!");
            m_CryptoViewer.StopService();
            m_CryptoViewer = null;
            AppHelper.Quit();
        }
    }

    private void PauseStockMarket()
    {
        Debug.Log("Stock market is wating for a response...");
        m_bPaused = true;
    }

    private void ResumeStockMarket()
    {
        Debug.Log("Stock market is now re-opened!");
        m_bPaused = false;
    }
}

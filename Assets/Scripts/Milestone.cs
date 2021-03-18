
public class Milestone
{
    private readonly decimal m_Threshold = 0;
    public float Points { get; private set; } = 0;

    public Milestone(decimal number, float points = 100f)
    {
        m_Threshold = number;
        Points = points;
    }

    public bool HasAchieved(decimal val)
    {
        return m_Threshold <= val;
    }
}

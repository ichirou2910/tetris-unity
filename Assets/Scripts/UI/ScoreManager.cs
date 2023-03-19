using Events;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private int score = 0;

    public void Score(int amount)
    {
        score += amount;
        scoreText.SetText("Score: " + score.ToString());
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this.SuscribeEvent(EventID.OnPlayerScore, (param) => Score((int)param));
    }
}

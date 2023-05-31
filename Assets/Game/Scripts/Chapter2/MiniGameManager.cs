using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject glassUI;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject miniGamePlayer;
    [SerializeField] private GameObject drink;
    public float sideBoundsDistance = 2.5f;

    private Vector3 drinkStartPos;
    public bool isRunning;
    public bool right;
    private Timer alternateTimer;

    [SerializeField] private Slider[] drinkSlider;
    [SerializeField] private TextMeshProUGUI[] aiTimes;
    [SerializeField] private TextMeshProUGUI winnerText;
    public bool aiIsDrinking;
    public int tougherAi;
    public float timeRunning;
    private bool showResults;

    private Animator uiAnimator;
    
    private void Awake()
    {
        uiAnimator = gameUI.GetComponent<Animator>();
        alternateTimer = GetComponent<Timer>();
        gameUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!showResults)
        {
            int playersFinished = 0;
            for (int x = 0; x < aiTimes.Length; x++)
            {
                if (aiTimes[x].text != "0.00")
                {
                    playersFinished++;
                    if (playersFinished == 3)
                    {
                        showResults = true;
                        WinConditions();
                        break;
                    }
                }
                
            }
        }
        
         // AI 10-30 per frame
         // 80000 total
         if (aiIsDrinking)
         {
             for (int i = 0; i < Random.Range(0, 6); i++)
             {
                 drinkSlider[1 - tougherAi].value--;
             }
             
             for (int i = 0; i < Random.Range(0, 8); i++)
             {
                 drinkSlider[tougherAi].value--;
             }

             for (int i = 0; i < aiTimes.Length; i++)
             {
                 if (drinkSlider[i].value == 0 && aiTimes[i].text == "0.00")
                 {
                     aiTimes[i].text = timeRunning.ToString("0.00");
                 }
             }
         }
             
         timeRunning += Time.deltaTime;

         if (isRunning)
         {
             // drinkSlider[0].value--;

             if (alternateTimer.isRunning)
             {
                 if (drink.transform.position.x < drinkStartPos.x - sideBoundsDistance)
                 {
                     right = true;
                 }
                 else if (drink.transform.position.x > drinkStartPos.x + sideBoundsDistance)
                 {
                     right = false;
                 }

                 if (right)
                 {
                     drink.transform.position += Time.deltaTime * Vector3.right;
                 }
                 else
                 {
                     drink.transform.position += Time.deltaTime * Vector3.left;
                 }
             }
             else
             {
                 alternateTimer.StartTimer(Random.Range(0.1f, 2f));
                 right = !right;
             }

             // key.transform.position += new Vector3(Input.GetAxis("Mouse X") * 0.7f, Input.GetAxis("Mouse Y") * 0.7f, 0);
         }
    }

    public void WinConditions()
    {
        Cursor.visible = true;
        isRunning = false;
        drink.transform.position = drinkStartPos;
        miniGamePlayer.SetActive(false);

        if (showResults)
        {
            // drink.SetActive(false);--
            uiAnimator.SetTrigger("Results");

            float[] timeTaken = new float[aiTimes.Length];

            for (int i = 0; i < aiTimes.Length; i++)
            {
                timeTaken[i] = float.Parse(aiTimes[i].text);
            }

            int winner = tougherAi;
            for (int i = 0; i < timeTaken.Length; i++)
            {
                if (timeTaken[i] == timeTaken.Min())
                {
                    winner = i;
                    print(winner);
                }
            }

            winnerText.text = "Winner : " + aiTimes[winner].transform.parent.GetChild(0).name;
            uiAnimator.SetInteger("Winner", winner);
        }
    }

    public void StartMiniGame()
    {
        int x = Random.Range(0, 2);
        print(x);
        if (x == 0)
        {
            tougherAi = 0;
        }
        else
        {
            tougherAi = 1;
        }

        startScreen.SetActive(false);
        Cursor.visible = false;
        isRunning = true;
        drinkStartPos = drink.transform.position;
        drink.SetActive(true);
        miniGamePlayer.SetActive(true);
        glassUI.SetActive(true);
    }

    public void EndMiniGame()
    {
        // Cursor.visible = true;
        // isRunning = false;
        // drink.SetActive(false);
        // miniGamePlayer.SetActive(false);
        gameUI.SetActive(false);
        glassUI.SetActive(false);
        gameObject.SetActive(false);
    }
}

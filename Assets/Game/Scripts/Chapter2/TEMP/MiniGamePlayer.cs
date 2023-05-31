using UnityEngine;
using UnityEngine.UI;

public class MiniGamePlayer : MonoBehaviour
{
    [SerializeField] public Slider playerDrinkSlider;
    [SerializeField] private MiniGameManager miniGameManager;
    
    private Vector3 initialPos;

    private void Awake()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        // for (int i = 0; i < Random.Range(10, 30); i++)
        // {
        //     playerDrinkSlider.value--;
        // }
        
        if (transform.position.x < initialPos.x - miniGameManager.sideBoundsDistance)
        {
            transform.position = new Vector3(initialPos.x - miniGameManager.sideBoundsDistance, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > initialPos.x + miniGameManager.sideBoundsDistance)
        {
            transform.position = new Vector3(initialPos.x + miniGameManager.sideBoundsDistance, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position += new Vector3(Input.GetAxis("Mouse X") * 0.5f, 0, 0);
        }
    }

    public void IsDrinking(ParticleSystem ps)
    {
        playerDrinkSlider.value--;

        if (playerDrinkSlider.value < playerDrinkSlider.maxValue)
        {
            miniGameManager.aiIsDrinking = true;
        }
        
        if (playerDrinkSlider.value == 0)
        {
            ps.loop = false;
            miniGameManager.WinConditions();
            // miniGameManager.EndMiniGame();
            // miniGameManager.isRunning = false;
            // print(miniGameManager.timeRunning);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        
    }
}

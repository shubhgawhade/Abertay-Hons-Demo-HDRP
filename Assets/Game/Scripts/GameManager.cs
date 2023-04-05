using UnityEngine;

public class GameManager : MonoBehaviour
{
    // public bool isMoveable;
    // public static bool IsMoveable;
    public static bool IsInteracting;
    public static string CurrentSpeaker;
    
    // public int health;
    public static float PlayerHealth;

    public int intelligence;
    public static int Intelligence;

    public bool testRay;
    public static bool TestRay;
    
    
    // TEXT READER VARIABLES

    // public static float DialogueSKipDelay;
    [SerializeField] public static float dialogueSkipDelay { get; set; }

    private void Awake()
    {
        // IsMoveable = isMoveable;
        Intelligence = intelligence;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // print($"PLAYER HEALTH: {PlayerHealth}");
        // print($" IS INTERACTING : {IsInteracting}");
        intelligence = Intelligence;
        TestRay = testRay;
        // print("IS INTERACTING: " + isInteracting);
    }
}

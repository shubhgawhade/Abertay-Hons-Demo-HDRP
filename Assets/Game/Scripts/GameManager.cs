using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isMoveable;
    public static bool IsMoveable = false;
    public static bool isInteracting;
    public static string CurrentSpeaker;

    public int intelligence;
    public static int Intelligence = 0;
    
    
    // TEXT READER VARIABLES

    // public static float DialogueSKipDelay;
    [SerializeField] public static float dialogueSkipDelay { get; set; }

    private void Awake()
    {
        IsMoveable = isMoveable;
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
        intelligence = Intelligence;
        // print("IS INTERACTING: " + isInteracting);
    }
}

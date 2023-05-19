using UnityEngine;
using UnityEngine.Serialization;

public class Chapters : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    [SerializeField] protected PlayerCharacterControl playerCharacterControl;
    [SerializeField] protected GameObject[] characters;
    [SerializeField] protected AICharacterControl[] aiCharacterControl;
    [SerializeField] protected Animator[] animators;
    [SerializeField] protected GameObject[] weapons;

    protected Animator playerAnimator;
    protected TextReader textReader;

    public bool scenePaused;
    public int sceneNum;
    
    protected virtual void Awake()
    {
        textReader = GetComponent<TextReader>();
        aiCharacterControl = new AICharacterControl[characters.Length];
        playerCharacterControl = player.GetComponent<PlayerCharacterControl>();
        animators = new Animator[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            aiCharacterControl[i] = characters[i].GetComponent<AICharacterControl>();
            animators[i] = characters[i].GetComponent<Animator>();
        }

        playerAnimator = player.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

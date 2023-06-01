using System;
using UnityEngine;

public class Chapters : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] protected PlayerCharacterControl playerCharacterControl;
    [SerializeField] public GameObject[] characters;
    [SerializeField] public AICharacterControl[] aiCharacterControl;
    [SerializeField] protected Animator[] animators;
    [SerializeField] protected GameObject[] weapons;

    protected Animator playerAnimator;
    protected TextReader textReader;

    public bool scenePaused;
    public int sceneNum;
    public bool positionSet;
    public bool scriptSet;
   
    public static Action SceneActive;

    protected virtual void Awake()
    {
        GameManager.ChaptersManager = this;
        
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
        
        // TELL THE GAME MANAGER THAT THE SCENE IS ACTIVE TO LOAD SAVED CHAPTER DATA
        if (GameManager.useSave)
        {
            SceneActive();
        }
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

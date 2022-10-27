using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Chapter1 : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] characters;
    [SerializeField] private AICharacterControl[] aiCharacterControl;
    [SerializeField] private Animator[] animators;

    public bool nextScene;
    private Animator playerAnimator;
    
    public enum Scene
    {
        InitialMove,
        InitialDialogue,
        PreFightAnims,
        KillDecision,
        PostDecisionAnimations,
        PostDecisionDialogue,
        SurvivorDecision,
        EOC
    }

    public Scene scene = Scene.InitialMove;

    private void Awake()
    {
        aiCharacterControl = new AICharacterControl[characters.Length];
        animators = new Animator[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            aiCharacterControl[i] = characters[i].GetComponent<AICharacterControl>();
            animators[i] = characters[i].GetComponent<Animator>();
        }

        playerAnimator = player.GetComponent<Animator>();

    }

    private void A()
    {
        
        print("STOPPED");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (scene)
        {
            case Scene.InitialMove:
                foreach (AICharacterControl item in aiCharacterControl)
                {
                    if (!item.aIStopped)
                    {
                        nextScene = false;
                        break;
                    }

                    scene++;
                }
                break;
            
            case Scene.InitialDialogue:
                GetComponent<TextReader>().ToggleUI();
                scene++;
                break;
            
            case Scene.PreFightAnims:
                break;
            
            case Scene.KillDecision:
                break;
            
            case Scene.PostDecisionAnimations:
                break;
            
            case Scene.PostDecisionDialogue:
                break;
            
            case Scene.SurvivorDecision:
                break;
            
            case Scene.EOC:
                break;
        }
        
        
        
        
        
        if(nextScene)
        {
            //PLAY DIALOGUE
            
            nextScene = false;

            // print("Scene Change");
            // playerAnimator.SetTrigger("DrawGun");
            // playerAnimator.SetTrigger("Grab");
        }


        /*
            Activate the looking at player 
            on mouse right-click.
        
        if (Input.GetMouseButtonDown(1))
        {
            LookAtPlayer();
            
        }
        */
    }

   
   
/*
    Angle characters in scene to look at the player.

    private void LookAtPlayer()
    {
        foreach (GameObject character in characters)
        {
            character.transform.LookAt(player.transform);
        }
    }
*/
}

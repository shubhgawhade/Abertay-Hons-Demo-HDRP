using UnityEngine;

public class Chapter2 : MonoBehaviour
{
    [SerializeField] private GameObject[] ai;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (GameObject go in ai)
            {
                go.SetActive(true);
            }
        }
    }
}

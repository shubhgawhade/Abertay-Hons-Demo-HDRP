using UnityEngine;

public class FloorTriggers : MonoBehaviour
{
    public static DetectFadeMaterials[] detectFadeMats;
    [SerializeField] private DetectFadeMaterials[] _detectFadeMaterials;
    
    private void Awake()
    {
        detectFadeMats = _detectFadeMaterials;
    }
}

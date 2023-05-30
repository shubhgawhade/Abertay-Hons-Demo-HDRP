using System.Collections;
using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    public TextMeshProUGUI damageText;

    private void Start()
    {
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.eulerAngles = new Vector3(-transform.eulerAngles.x, 180, 0);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}

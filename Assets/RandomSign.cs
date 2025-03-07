using UnityEngine;

public class RandomSign : MonoBehaviour
{
    public GameObject[] signs;
    public GameObject activeSign;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssignRandomSign();
    }

    public void AssignRandomSign()
    {
        if (signs.Length > 0)
        {
            foreach (var sign in signs)
            {
                if (sign != null)
                {
                    sign.SetActive(false);
                }
            }
            int randomIndex = Random.Range(0, signs.Length);
            activeSign = signs[randomIndex];

            if (activeSign != null) activeSign.SetActive(true);
        }
    }
}

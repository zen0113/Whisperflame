using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static int maxHealth = 3;
    public int currentHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Image img in hearts)
        {
            img.sprite = emptyHeart; // 모든 하트를 빈 하트로 초기화
        }
        for (int i = 0; i < currentHealth; i++)
        {
            hearts[i].sprite = fullHeart; // 현재 체력만큼만 하트를 채움
        }
    }
    public void Damaged()
    {
        currentHealth--;
    }
}

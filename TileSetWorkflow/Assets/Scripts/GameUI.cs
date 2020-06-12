using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameUI : MonoBehaviour
{
    public Slider healthSlider;
    public Gradient healthGradient;
    public Image healthFill;
    public Image cooldownChargeFill;
    public Image chargeFill;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject winScreenPanel;
    public GameObject keyFoundText;
    private bool m_GamePaused = false;
    private bool m_GameIsPausable;
    private void Awake()
    {
        Time.timeScale = 1;
    }
    void Start()
    {
        m_GameIsPausable = true;
        Cursor.visible = false;
        //if (m_GamePaused)
        //{
        //    resumeGame();
        //}
        gameOverPanel.SetActive(m_GamePaused);
        pauseMenuPanel.SetActive(m_GamePaused);
        //print(keyFoundText.GetComponent<TextMeshProUGUI>().text);
    }
    void Update()
    {
        if (Input.GetButtonDown("Escape") && m_GameIsPausable)
        {
            if (m_GamePaused)
                resumeGame();
            else
                pauseGame();
        }
    }
    public void showGameOverPanel()
    {
        Cursor.visible = true;
        //if (m_GamePaused)
        //{
        //    pauseMenuPanel.SetActive(false);
        //}
        gameOverPanel.SetActive(true);
    }
    public void showWinScreenPanel()
    {
        Cursor.visible = true;
        winScreenPanel.SetActive(true);
    }
    void pauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0f;
        m_GamePaused = true;
    }
    public void resumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1f;
        m_GamePaused = false;
    }

    public void setMaxHealth(int maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = maxHP;
        healthFill.color = healthGradient.Evaluate(1f);
    }

    public void setCurrentHealth(int health)
    {
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }

    public void setCurrentCharge(float charge)
    {
        chargeFill.fillAmount = charge;
    }

    public void setChargeCooldown(float cooldown)
    {
        
        if (cooldownChargeFill.fillAmount <=50 && cooldownChargeFill.fillAmount > 0)
        {
            cooldownChargeFill.fillAmount = cooldown;
        }
        else
        {
            cooldownChargeFill.fillAmount = 0f;
        }
       
    }
    public void initialiseChargeCooldown()
    {
        cooldownChargeFill.fillAmount = 0.5f;
    }
    public void setKeyFoundText(bool isFound)
    {
        keyFoundText.SetActive(isFound);
    }
    public bool getPauseStatus() { return m_GamePaused; }
    public bool getPausableStatus() { return m_GameIsPausable; }
    public void setPausableStatus( bool status) { m_GameIsPausable = status; }
}

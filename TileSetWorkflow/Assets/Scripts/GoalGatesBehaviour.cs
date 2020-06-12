using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class GoalGatesBehaviour : MonoBehaviour
{
    private const string PLAYER_HAS_KEY_MSG = "Press 'E' to exit the level.";
    private const string DEFAULT_GATE_MSG = "You need the key to exit the level";
    public Player m_Player;
    public TextMeshProUGUI m_GoalText;
    private CanvasGroup m_GoalCanvasGroup;
    private void Awake()
    {
        m_GoalCanvasGroup = this.GetComponentInChildren<CanvasGroup>();
    }
    void Start()
    {
        hideGoalText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void showGoalText()
    {
        if (m_Player.getKeyStatus())
            m_GoalText.text = PLAYER_HAS_KEY_MSG;
        else
            m_GoalText.text = DEFAULT_GATE_MSG;

        m_GoalCanvasGroup.alpha = 1;
    }
    private void hideGoalText()
    {
        m_GoalCanvasGroup.alpha = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.CompareTag("Player"))
            {
                showGoalText();
            }
        }
        else
            return;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.CompareTag("Player"))
            {
                // if player has the key
                if (m_Player.getKeyStatus())
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // pop up the appropriate UI to proceed back to the menu
                        m_Player.onPlayerWin();
                    }
                }
                else
                    return;      
            }
        }
        else
            return;
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (collision.CompareTag("Player"))
            {
                hideGoalText();
            }
        }
        else
            return;
    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public string playerName;
    public List<Card> hand = new List<Card>();

    public Image card1;
    public Image card2;

    public TextMeshProUGUI playerText;

    public TextMeshProUGUI playerPot;

    public TextMeshProUGUI playerBet;

    private int pot = 100;

    public bool isAI;
    private bool folded = false;


    private void Start()
    {
        playerText.text = playerName;
        playerPot.text = "$"+pot.ToString();
    }

    public void AddCard(Card cardInstance, int index)
    {
        if (cardInstance == null)
        {
            Debug.LogError("Card instance is null!");
            return;
        }

        if (index == 0)
        {
            card1.sprite = cardInstance.GetComponent<SpriteRenderer>().sprite;
        }else if (index == 1)
        {
            card2.sprite = cardInstance.GetComponent<SpriteRenderer>().sprite;
        }

        // Store the card in the player's hand
        hand.Add(cardInstance);
    }

    public void Fold()
    {
        folded = true;
        //Debug.Log(playerName + " has folded.");
        GameManager.instance.NextTurn();
    }

    public void Call()
    {
        // Implement call logic here
        //Debug.Log(playerName + " has called.");
        UpdatePlayerPot(GameManager.instance.minBet);
        GameManager.instance.UpdatePot(1);
        GameManager.instance.NextTurn();
    }

    public void Raise()
    {
        // Implement raise logic here
        //Debug.Log(playerName + " has raised.");
        int bet = GameManager.instance.minBet * 2;
        UpdatePlayerPot(bet);
        GameManager.instance.UpdatePot(2);
        GameManager.instance.NextTurn();
    }

    public void Check()
    {
        // Implement check logic here
        // Debug.Log(playerName + " has raised.");
        GameManager.instance.NextTurn();
    }

    public bool HasFolded()
    {
        return folded;

    }

    public void PerformAIAction()
    {
        int action = PokerHandEvaluator.EvaluateBestMove(hand, GameManager.instance.communityCards);

        Debug.Log($"{playerName} Action: " + action);
        
        switch (action)
        {
            case 0:
                Fold();
                break;
            case 1:
                Call();
                break;
            case 2:
                Raise();
                break;
        }
    }

    public void UpdatePlayerText(string newText)
    {
        playerText.text = newText;
    }

    public void UpdatePlayerPot(int bet)
    {
        playerBet.text = "$"+bet.ToString();
        pot -= bet;
        playerPot.text = "$"+pot.ToString();
    }

}
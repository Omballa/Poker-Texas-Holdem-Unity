using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public int numAIOpponents = 2;
    public Player player;

    [SerializeField] private List<GameObject> allCardPrefabs; // Store all card prefabs
    private List<GameObject> deck = new List<GameObject>(); // Store shuffled deck as prefabs

    public List<Player> allAIPlayers;
    private List<Player> allPlayers = new List<Player>();

    public int currentTurn = 0;
    public int currentRound = 1;
    public bool isClockwise = true;
    public bool roundInProgress = false;
    public List<Image> communityCardsImage;

    public List<Card> communityCards;

    public TextMeshProUGUI potText;

    public int pot = 10;

    public int minBet = 5;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializePlayers();
        InitializeDeck();
        DealInitialCards();
        StartRound();
        potText.text = "$ " + pot.ToString();
    }

    void InitializePlayers()
    {
        allPlayers.Clear();

        allPlayers.Add(player);

        for (int i = 0; i < numAIOpponents && i < allAIPlayers.Count; i++)
        {
            Player aiPlayer = allAIPlayers[i];
            aiPlayer.gameObject.SetActive(true);
            allPlayers.Add(aiPlayer);
        }
    }

    void DealInitialCards()
    {
        for (int i = 0; i < 2; i++) // Give each player 2 cards
        {
            foreach (Player player in allPlayers)
            {
                GameObject cardObject = DrawCard(); // Get a card prefab
                if (cardObject != null)
                {
                    Card cardComponent = cardObject.GetComponent<Card>();
                    if (cardComponent != null)
                    {
                        player.AddCard(cardComponent , i);
                    }
                    else
                    {
                        Debug.LogError("Drawn card prefab is missing the Card component!");
                    }
                }
                else
                {
                    Debug.LogError("Deck is empty or DrawCard() returned null!");
                }
            }
        }
    }

    void InitializeDeck()
    {
        deck.Clear();
        deck.AddRange(allCardPrefabs);
        ShuffleDeck();
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            GameObject temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public GameObject DrawCard()
    {
        if (deck.Count == 0)
        {
            InitializeDeck();
        }

        if (deck.Count > 0)
        {
            GameObject drawnCardPrefab = deck[0];
            deck.RemoveAt(0);
            return drawnCardPrefab;
        }
        else
        {
            Debug.LogError("Deck is empty!");
            return null;
        }
    }

    void StartRound()
    {
        Debug.Log($"Starting round {currentRound}...");
        
        currentTurn = 0;
        roundInProgress = true;
        //NextTurn();
    }

    IEnumerator AITakeTurn(Player aiPlayer)
    {
        yield return new WaitForSeconds(1.0f); // Wait before action
        aiPlayer.PerformAIAction();
        yield return new WaitForSeconds(4.0f); // Wait after action
        //AdvanceToNextTurn();
    }

    void AdvanceToNextTurn()
    {
        do {
            currentTurn++;
            if (currentTurn >= allPlayers.Count)
            {
                EndRound();
                return;
            }
        } while (allPlayers[currentTurn].HasFolded()); // Skip folded players

        if (allPlayers[currentTurn].isAI)
        {
            StartCoroutine(AITakeTurn(allPlayers[currentTurn]));
        }
        else
        {
            Debug.Log("Player's turn");
            // Here you would enable player input controls
        }
    }

    public void NextTurn()
    {
        if (!roundInProgress) return;
        
        AdvanceToNextTurn();
    }
    void EndRound()
    {
        roundInProgress = false;
        Debug.Log($"Round {currentRound} ended. Starting next round...");

        if (currentRound == 1){
            //Deal 3 cards at the community spawn point. Separate by 1f
            for (int i = 0; i < 3; i++)
            {
                GameObject cardObject = DrawCard();
                if (cardObject != null)
                {
                    Card cardComponent = cardObject.GetComponent<Card>();
                    if (cardComponent != null)
                    {
                        communityCardsImage[i].gameObject.SetActive(true);
                        communityCardsImage[i].sprite = cardComponent.GetComponent<SpriteRenderer>().sprite;
                        communityCards.Add(cardComponent);
                    }
                    else
                    {
                        Debug.LogError("Drawn card prefab is missing the Card component!");
                    }
                }
                else
                {
                    Debug.LogError("Deck is empty or DrawCard() returned null!");
                }
            }
            
        }
        else if(currentRound == 2){
            GameObject cardObject = DrawCard();
            if (cardObject != null)
            {
                Card cardComponent = cardObject.GetComponent<Card>();
                if (cardComponent != null)
                {
                    communityCardsImage[3].gameObject.SetActive(true);
                    communityCardsImage[3].sprite = cardComponent.GetComponent<SpriteRenderer>().sprite;
                    communityCards.Add(cardComponent);
                }
                else
                {
                    Debug.LogError("Drawn card prefab is missing the Card component!");
                }
            }
            else
            {
                Debug.LogError("Deck is empty or DrawCard() returned null!");
            }
        }
        currentRound++;
        StartRound();
    }

    public void UpdatePot(int action)
    {
        if (action == 1)
        {
            pot += minBet;
        }

        if (action == 2)
        {
            pot += minBet * 2;
        }
        potText.text = "$ " + pot.ToString();
    }
}
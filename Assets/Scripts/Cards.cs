using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour
{
    public enum Suit {Clubs,Diamonds,Spades,Hearts}

    public Suit suit;

    public string value;

    // Converts the face card values (J, Q, K, A) to numerical values
    public int GetCardValue()
    {
        switch (value)
        {
            case "A": return 14; 
            case "K": return 13;
            case "Q": return 12;
            case "J": return 11;
            default:
                int numericValue;
                return int.TryParse(value, out numericValue) ? numericValue : 0;
        }
    }
}
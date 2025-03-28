using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokerHandEvaluator
{
    public enum HandRank
    {
        HighCard = 1,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public static int EvaluateBestMove(List<Card> hand, List<Card> communityCards)
    {
        if (hand == null || communityCards == null || hand.Count != 2 || communityCards.Count < 3)
        {
            // Either call or raise when in the first round...
            return Random.Range(1, 3);
        }

        List<Card> allCards = new List<Card>(hand);
        allCards.AddRange(communityCards);
        allCards = allCards.OrderByDescending(card => card.GetCardValue()).ToList(); // Sort by value

        HandRank bestHand = DetermineBestHand(allCards);

        // Decision logic based on hand strength
        if (bestHand >= HandRank.Flush) return 2; // Raise if Flush or better
        if (bestHand >= HandRank.TwoPair) return 1; // Call if Two Pair, Three of a Kind, or Straight
        return 0; // Fold otherwise
    }

    private static HandRank DetermineBestHand(List<Card> cards)
    {
        if (IsRoyalFlush(cards)) return HandRank.RoyalFlush;
        if (IsStraightFlush(cards)) return HandRank.StraightFlush;
        if (IsFourOfAKind(cards)) return HandRank.FourOfAKind;
        if (IsFullHouse(cards)) return HandRank.FullHouse;
        if (IsFlush(cards)) return HandRank.Flush;
        if (IsStraight(cards)) return HandRank.Straight;
        if (IsThreeOfAKind(cards)) return HandRank.ThreeOfAKind;
        if (IsTwoPair(cards)) return HandRank.TwoPair;
        if (IsOnePair(cards)) return HandRank.OnePair;
        return HandRank.HighCard;
    }

    private static bool IsRoyalFlush(List<Card> cards) => IsStraightFlush(cards) && cards.Any(c => c.value == "A");

    private static bool IsStraightFlush(List<Card> cards) => IsFlush(cards) && IsStraight(cards);

    private static bool IsFourOfAKind(List<Card> cards) => cards.GroupBy(c => c.value).Any(group => group.Count() == 4);

    private static bool IsFullHouse(List<Card> cards)
    {
        var grouped = cards.GroupBy(c => c.value).ToList();
        return grouped.Any(g => g.Count() == 3) && grouped.Any(g => g.Count() == 2);
    }

    private static bool IsFlush(List<Card> cards) => cards.GroupBy(c => c.suit).Any(group => group.Count() >= 5);

    private static bool IsStraight(List<Card> cards)
    {
        List<int> values = cards.Select(c => c.GetCardValue()).Distinct().ToList();
        values.Sort();

        for (int i = 0; i <= values.Count - 5; i++)
        {
            if (values[i + 4] - values[i] == 4) return true;
        }

        return values.Contains(14) && values.Take(4).SequenceEqual(new List<int> { 2, 3, 4, 5 });
    }

    private static bool IsThreeOfAKind(List<Card> cards) => cards.GroupBy(c => c.value).Any(group => group.Count() == 3);

    private static bool IsTwoPair(List<Card> cards) => cards.GroupBy(c => c.value).Count(g => g.Count() == 2) >= 2;

    private static bool IsOnePair(List<Card> cards) => cards.GroupBy(c => c.value).Any(group => group.Count() == 2);
}

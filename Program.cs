using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinsOnClock
{
    class Program
    {
        static int[] coinWeights = new int[] { 1, 5, 10 };
        static int maxSlotsCount = 12;
        static Dictionary<int, int> coinsOnClock = new Dictionary<int, int>();
        static int iteration = 1;
        static void Main(string[] args)
        {
            Console.WriteLine("Analysis is started...");
            PutCoinsOnClock(1);
        }

        static void LogState()
        {
            Console.WriteLine($"Iteration: {iteration}");
            Console.WriteLine($"Slots: { string.Join(",", coinsOnClock.Keys)}");
            Console.WriteLine($"Choices: { string.Join(",", coinsOnClock.Values)}");
            Console.WriteLine($"Remaining Coins: { GetSumOfRemainingCoinWeights()}");
            Console.WriteLine($"\n\r");
        }

        static void PutCoinsOnClock(int currentPosition, int startCoinWeightIndex = 0)
        {
            // while (iteration != 474)
            while (coinsOnClock.Count() < maxSlotsCount)
            {
                iteration += 1;

                if (currentPosition == maxSlotsCount)
                {
                  coinsOnClock.Add(currentPosition, GetSumOfRemainingCoinWeights());  
                  break;
                }

                // loop over coinWeights (1 -> 5 -> 10)
                for (var coinWeightIndex = startCoinWeightIndex; coinWeightIndex < coinWeights.Length; coinWeightIndex++)
                {
                    var coinWeight = coinWeights[coinWeightIndex];

                    // We can't use more than certain amount of Coins with the same Weight
                    // if occupied - decrease weight 1 -> 5 -> 10 and try again
                    if (ReachedCoinWeightUsageLimit(coinWeight) || isNextSlotOccupied(currentPosition, coinWeight))
                    {
                        if (coinWeight != coinWeights.Max())
                            continue;

                        // Handle the case when we have gone through all coinWeights, but couldn't find a way to proceed:
                        // get back to previous Choice, decrease coinWeight and repeat Iteration until we go further
                        int prevPosition = 0, prevPositionCoinWeight = 0;
                        GoBack(out prevPosition, out prevPositionCoinWeight);

                        while (prevPositionCoinWeight == coinWeights.Max())
                        {
                            GoBack(out prevPosition, out prevPositionCoinWeight);
                        }

                        var prevPositionCoinWeightIdx = Array.IndexOf(coinWeights, prevPositionCoinWeight);
                        // PutCoinsOnClock(prevPosition, prevPositionCoinWeightIdx + 1);

                        currentPosition = prevPosition;
                        startCoinWeightIndex = prevPositionCoinWeightIdx + 1;
                    }
                    else
                    {
                        // Save CoinWeight choice for the current Slot  
                        coinsOnClock.Add(currentPosition, coinWeight);
                        // Move on to new position after CoinWeight is applied
                        // PutCoinsOnClock(getNextPosition(currentPosition, coinWeight));
                        currentPosition = getNextPosition(currentPosition, coinWeight);
                        startCoinWeightIndex = 0;
                    }

                    break;
                }
            }

            LogState();
        }

        static void GoBack(out int prevPosition, out int prevPositionCoinWeight)
        {
            prevPosition = coinsOnClock.Last().Key;
            prevPositionCoinWeight = coinsOnClock.Last().Value; // {10 | 5 | 1}
            coinsOnClock.Remove(coinsOnClock.Last().Key);
        }

        static int GetSumOfRemainingCoinWeights()
        {
            return coinWeights.Aggregate((result, coinWeight) => result + coinWeight) * 4 - coinsOnClock.Values.Sum();
        }

        static int getNextPosition(int currentPosition, int coinWeight)
        {
            var newSlotPosition = (currentPosition + coinWeight) % maxSlotsCount;
            if (newSlotPosition == 0)
                newSlotPosition = maxSlotsCount;

            return newSlotPosition;
        }

        static bool isNextSlotOccupied(int currentPosition, int coinWeight)
        {
            var newSlotPosition = getNextPosition(currentPosition, coinWeight); // 1..12

            return coinsOnClock.ContainsKey(newSlotPosition);
        }

        static bool ReachedCoinWeightUsageLimit(int coinWeight)
        {
            return coinsOnClock.Values.Count(choice => choice == coinWeight) == 4;
        }
    }
}

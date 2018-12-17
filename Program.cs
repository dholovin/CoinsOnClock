using System;
using System.Collections.Generic;
using System.Linq;

namespace CoinsOnClock
{
    class Program
    {
        // TODO: consider combining slots and choices, if possible  
        static bool[] slots = new bool[12];
        static int[] choices = new int[12];
        static List<int> positionHistory = new List<int>();
        static int[] coinWeights = new int[] { 10, 5, 1 };

        // static int remainingCoinWeightsSum =>
        //     (coinWeights.Aggregate((result, coinWeight) => result + coinWeight) * 4)
        //     - choices.Aggregate((result, choice) => result + choice);



        static Dictionary<int, int> coinsOnClock = new Dictionary<int, int>();
        static int iteration = 1;
        static void Main(string[] args)
        {
            Console.WriteLine("Analysis is started...");

            PutCoinsOnClock(1);
        }

        static void LogState()
        {
            Console.WriteLine($"Slots: { string.Join(",", coinsOnClock.Keys)}");
            Console.WriteLine($"Choices: { string.Join(",", coinsOnClock.Values)}");
            Console.WriteLine(GetSumOfRemainingCoinWeights());
        }

        static void PutCoinsOnClock(int currentPosition, int startCoinWeightIndex = 0)
        {
            LogState();

            Console.WriteLine($"Iteration: {iteration}");
            iteration += 1;

            if (coinsOnClock.Count == 12)
                return;

            // loop over coinWeights (10 -> 5 -> 1)
            for (var coinWeightIndex = startCoinWeightIndex; coinWeightIndex < coinWeights.Length; coinWeightIndex++)
            {
                LogState();

                var coinWeight = coinWeights[coinWeightIndex];

                // We can't use more than certain amount of Coins with the same Weight
                // if occupied - decrease weight 10 -> 5 -> 1 and try again
                if (ReachedCoinWeightUsageLimit(coinWeight) || isNextSlotOccupied(currentPosition, coinWeight))
                {
                    if (coinWeight != coinWeights.Min())
                        continue;

                    // Handle the case when we have gone through all coinWeights, but couldn't find a way to proceed:
                    // get back to previous Choice, decrease coinWeight and repeat Iteration until we go further
                    int prevPosition = 0, prevPositionCoinWeight = 0;
                    GoBack(out prevPosition, out prevPositionCoinWeight);
                   
                    while (prevPositionCoinWeight == coinWeights.Min())
                    {
                        // if coin weight on prev step was '1' - can't decrease - GoBack again
                        GoBack(out prevPosition, out prevPositionCoinWeight);
                    }

                    var prevPositionCoinWeightIdx = Array.IndexOf(coinWeights, prevPositionCoinWeight);
                    PutCoinsOnClock(prevPosition, prevPositionCoinWeightIdx + 1);
                }
                else
                {
                    // Save CoinWeight choice for the current Slot  
                    coinsOnClock.Add(currentPosition, coinWeight);
                    // Move on to new position after CoinWeight is applied
                    PutCoinsOnClock(getNextPosition(currentPosition, coinWeight));
                }

            }
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
            var newSlotPosition = (currentPosition + coinWeight) % 12;
            if (newSlotPosition == 0)
                newSlotPosition = 12;

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

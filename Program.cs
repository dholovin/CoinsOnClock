using System;
using System.Collections.Generic;
using System.Linq;

namespace ClockCoins
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

        static void Main(string[] args)
        {
            Console.WriteLine("Analysis is started...");

            var startPosition = 1;
            CalculateCoinWeightForClockPosition(startPosition);

            Console.WriteLine($"Slots: { string.Join(",", slots)}");
            Console.WriteLine($"Choices: { string.Join(",", choices)}");
            Console.WriteLine($"Position History: { string.Join(",", positionHistory)}");
            
            Console.WriteLine(GetRemainingCoinWeightsSum());

        }

        static void CalculateCoinWeightForClockPosition(int currentPosition)
        {
            if (GetRemainingCoinWeightsSum() == 0)
                return;

            foreach (var coinWeight in coinWeights)
            {
                if (reachedCoinWeightUsageLimit(choices, coinWeight))
                    continue; // We can't use more than certain amount of Coins with the same Weight

                if (isSlotOccupied(currentPosition, coinWeight))
                    continue; // if occupied - decrease weight 10 -> 5 -> 1 and try again
                //else if (isSlotOccupied(currentPosition, GetRemainingCoinWeightsSum()))
                    // TODO: try to move on without this validation
                    // continue; // if occupied - decrease weight 10 -> 5 -> 1 and try again
                else
                {
                    // Save CoinWeight choice for the current Slot                    
                    slots[currentPosition - 1] = true;
                    choices[currentPosition - 1] = coinWeight;
                    positionHistory.Add(currentPosition);

                    // Move on to new position after CoinWeight is applied
                    CalculateCoinWeightForClockPosition(getNextPosition(currentPosition, coinWeight));
                }
            }

            // Handle the case when we have gone through all coinWeights, but couldn't find a way to proceed:
            // get back to previous Choice, decrease coinWeight and repeat Iteration for that Slot until we go further
            // foreach (var prevPosition in reversedPositionHistory)
            var lastPositionHistoryIndex =  positionHistory.Count() - 1;
            for (var idx = lastPositionHistoryIndex; idx >= 0; idx--)
            {
                var prevPosition = positionHistory[idx];
                var prevPositionCoinWeight = choices[prevPosition - 1]; // {10 | 5 | 1}

                slots[prevPosition - 1] = false;
                choices[prevPosition - 1] = 0;
                positionHistory.RemoveAt(idx);

                if (prevPositionCoinWeight == coinWeights.Min())
                    // can't decrease CoinWeight - go back one more step and try to decrease coin weight there
                    continue;
                else
                {
                    var prevSlotCoinWeightIdx = Array.IndexOf(coinWeights, prevPositionCoinWeight);
                    
                    if (prevSlotCoinWeightIdx == -1)
                        throw new Exception(
                            $"Couldnt find coinWeight {prevPositionCoinWeight} in array {string.Join(",", coinWeights)}");
                    
                    // Decrease CoinWeight on the previous step and retry
                    CalculateCoinWeightForClockPosition(
                        getNextPosition(prevPosition, coinWeights[prevSlotCoinWeightIdx]));
                }
            }

            throw new Exception("No Answer Was Identified");
        }

        static int GetRemainingCoinWeightsSum() {
            return 64 // coinWeights.Aggregate((result, coinWeight) => result + coinWeight) * 4
            - choices.Sum();
        }

        static int getNextPosition(int currentPosition, int coinWeight)
        {
            var newSlotPosition = (currentPosition + coinWeight) % 12;
            if (newSlotPosition == 0)
                newSlotPosition = 12;

            return newSlotPosition;
        }

        static bool isSlotOccupied(int currentPosition, int coinWeight)
        {
            var newSlotPosition = getNextPosition(currentPosition, coinWeight); // 1..12

            // If there is something in that Slot - occupied
            return slots[newSlotPosition - 1] != false;
            // return Array.IndexOf(slots, newSlotPosition) != -1;
        }

        static bool reachedCoinWeightUsageLimit(int[] choices, int coinWeight)
        {
            return choices.Count(choice => choice == coinWeight) >= 4;
        }
    }
}

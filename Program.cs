using System;
using System.Linq;

namespace ClockCoins
{
    class Program
    {
        // TODO: consider combining slots and choices, if possible
        static bool[] slots = new bool[12];
        static int[] choices = new int[12];
        static int[] coinWeights = new int[] { 10, 5, 1 };

        static int remainingCoinWeightsSum =>
            (coinWeights.Aggregate((result, coinWeight) => result + coinWeight) * 4)
            - choices.Aggregate((result, choice) => result + choice);

        static void Main(string[] args)
        {
            Console.WriteLine("Analysis is started...");

            var startPosition = 1;
            CalculateCoinWeightForClockPosition(startPosition);

            Console.WriteLine($"Slots: { string.Join(",", slots)}");
            Console.WriteLine($"Choices: { string.Join(",", choices)}");
            Console.WriteLine(remainingCoinWeightsSum);

        }

        static void CalculateCoinWeightForClockPosition(int currentPosition)
        {
            if (remainingCoinWeightsSum == 0)
                return;

            foreach (var coinWeight in coinWeights)
            {
                if (choices.Count(choice => choice == coinWeight) == 4)
                    continue; // We can't use more than 4 items of the same Weight

                if (isSlotOccupied(currentPosition, coinWeight))
                    continue; // if occupied - decrease weight 10 -> 5 -> 1 and try again
                else if (isSlotOccupied(currentPosition, remainingCoinWeightsSum))
                    // TODO: try to move on without this validation
                    continue; // if occupied - decrease weight 10 -> 5 -> 1 and try again
                else
                {
                    // Save CoinWeight choice for the current Slot
                    slots[currentPosition - 1] = true;
                    choices[currentPosition - 1] = coinWeight;

                    // Move on to new position after CoinWeight is applied
                    CalculateCoinWeightForClockPosition(getNextPosition(currentPosition, coinWeight));
                }
            }

            // Handle the case when we have gone through all coinWeights, but couldn't find a way to proceed:
            // get back to previous Choice, decrease coinWeight and repeat Iteration for that Slot until we go further

            for (var prevSlotPositionIndex = currentPosition - 1 - 1; prevSlotPositionIndex != 0; prevSlotPositionIndex -= 1)
            {
                slots[prevSlotPositionIndex] = false;
                choices[prevSlotPositionIndex] = 0;

                var prevSlotCoinWeight = choices[prevSlotPositionIndex]; // {10 | 5 | 1}
                if (prevSlotCoinWeight == coinWeights.Min())
                    // can't decrease CoinWeight - go back one more step and try to decrease coin weight there
                    continue;
                else
                {
                    var prevSlotCoinWeightIdx = Array.IndexOf(coinWeights, prevSlotCoinWeight);
                    
                    if (prevSlotCoinWeightIdx == -1)
                        throw new Exception(
                            $"Couldnt find coinWeight {prevSlotCoinWeight} in array {string.Join(",", coinWeights)}");
                    
                    // Decrease CoinWeight on the previous step and retry
                    CalculateCoinWeightForClockPosition(
                        getNextPosition(prevSlotPositionIndex + 1, coinWeights[prevSlotCoinWeightIdx]));
                }
            }

            throw new Exception("No Answer Was Identified");
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
            var newSlotPosition = getNextPosition(currentPosition, coinWeight);

            // If there is something in that Slot - occupied
            return slots[newSlotPosition - 1] != false;
        }
    }
}

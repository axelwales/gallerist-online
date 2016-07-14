using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class InfluenceManager
    {
        public static int[] InfluenceToMoney = { 0, 1, 4, 8, 12, 15, 18, 20, 22, 24, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 };

        public static bool HasInfluenceAsMoney(this Player player, int amount)
        {
            if (amount == 0)
                return true;

            if(amount - 1 >= InfluenceToMoney.Count())
            {
                return false;
            }

            if(InfluenceToMoney[amount - 1] > player.Influence)
            {
                return false;
            }

            return true;
        }

        public static int GetMaxMoneyFromInfluence(this Player player)
        {
            return GetMaxMoneyFromInfluence(player.Influence);
        }

        public static int GetMaxMoneyFromInfluence(int influence)
        {
            int max = 0;
            while (InfluenceToMoney[max] < influence) { max++; }
            return max;
        }

        public static void UseInfluenceAsMoney(this Player player, int amount)
        {
            if (amount != 0 && player.HasInfluenceAsMoney(amount))
            {
                int max = player.GetMaxMoneyFromInfluence();
                player.Influence = InfluenceToMoney[max - amount];
            }
        }

        public static bool HasInfluenceAsFame(this Player player, int amount)
        {
            if(player.Influence/5 < amount - 1)
            {
                return false;
            }
            return true;
        }

        public static int GetMaxFameFromInfluence(this Player player)
        {
            return GetMaxFameFromInfluence(player.Influence);
        }

        public static int GetMaxFameFromInfluence(int influence)
        {
            int max = 0;
            while (max * 5 < influence) { max++; }
            return max;
        }


        public static void UseInfluenceAsFame(this Player player, int amount)
        {
            if(amount != 0 && player.HasInfluenceAsFame(amount))
            {
                int max = player.GetMaxFameFromInfluence();
                player.Influence = 5 * (max - amount);
            }
        }

        public static int CalculateFameGainedByInfluence(this Player player, int endInfluence)
        {
            return CalculateFameGainedByInfluence(player.Influence, endInfluence);
        }

        public static int CalculateFameGainedByInfluence(int playerInfluence, int space)
        {
            if (space % 5 != 0)
            {
                return 0;
            }
            return GetMaxFameFromInfluence(playerInfluence) - (space / 5);
        }

        public static int CalculateMoneySpentByInfluence(this Player player, int space)
        {
            return CalculateMoneySpentByInfluence(player.Influence, space);
        }

        public static int CalculateMoneySpentByInfluence(int startInfluence, int endInfluence)
        {
            if (!InfluenceToMoney.Contains(endInfluence))
            {
                return 0;
            }
            int amount = 0;
            while (InfluenceToMoney[amount] != endInfluence) { amount++; }
            return GetMaxMoneyFromInfluence(startInfluence) - amount;
        }
    }
}
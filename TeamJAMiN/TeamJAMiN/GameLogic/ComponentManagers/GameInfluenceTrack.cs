using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class GameInfluenceTrack
    {
        public static int[] InfluenceToMoney = { 0, 1, 4, 8, 12, 15, 18, 20, 22, 24, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35 };

        public static bool HasInfluenceAsMoney(this Player player, int amount)
        {
            if(amount >= InfluenceToMoney.Count())
            {
                return false;
            }

            if(InfluenceToMoney[amount] > player.Influence)
            {
                return false;
            }

            return true;
        }

        public static int GetMaxMoneyFromInfluence(this Player player)
        {
            int max = 0;
            while (InfluenceToMoney[max] < player.Influence) { max++; }
            return max;
        }

        public static void UseInfluenceAsMoney(this Player player, int amount)
        {
            if (player.HasInfluenceAsMoney(amount))
            {
                int max = player.GetMaxMoneyFromInfluence();
                player.Influence = InfluenceToMoney[max - amount];
            }
        }

        public static bool HasInfluenceAsFame(this Player player, int amount)
        {
            if(player.Influence/5 < amount)
            {
                return false;
            }
            return true;
        }

        public static int GetMaxFameFromInfluence(this Player player)
        {
            int max = 0;
            while (max*5 < player.Influence) { max++; }
            return max;
        }

        public static void UseInfluenceAsFame(this Player player, int amount)
        {
            if(player.HasInfluenceAsFame(amount))
            {
                int max = player.GetMaxFameFromInfluence();
                player.Influence = 5 * (max - amount);
            }
        }
    }
}
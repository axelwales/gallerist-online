using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.Controllers.GameControllerHelpers;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public static class ContractManager
    {
        public static Dictionary<GameContractLocation, GameActionState> PlayerBoardLocationToTicketState = new Dictionary<GameContractLocation, GameActionState>
        {
            { GameContractLocation.Any, GameActionState.ChooseTicketAny },
            { GameContractLocation.Investor, GameActionState.GetTicketInvestor },
            { GameContractLocation.Vip, GameActionState.GetTicketVip }
        };

        public static Dictionary<GameContractLocation, PlayerAssistantLocation> ContractToAssistant = new Dictionary<GameContractLocation, PlayerAssistantLocation>
        {
            { GameContractLocation.Any, PlayerAssistantLocation.ContractAny },
            { GameContractLocation.Investor, PlayerAssistantLocation.ContractInvestor },
            { GameContractLocation.Vip, PlayerAssistantLocation.ContractVip }
        };

        public static bool IsContractLocationEmpty(this Game game, GameContractLocation location)
        {
            return !game.Contracts.Any(c => c.Location == location) == false;
        }

        public static bool IsContractLocationEmpty(this Player player, GameContractLocation location)
        {
            return player.Contracts.Any(c => c.Location == location) == false;
        }

        public static bool HasAvailableContractLocation( this Player player)
        {
            var playerLocations = new List<GameContractLocation> { GameContractLocation.Vip, GameContractLocation.Investor, GameContractLocation.Any };
            foreach ( var location in playerLocations)
            {
                if (IsContractLocationEmpty(player, location))
                    return true;
                var contract = player.Contracts.First(c => c.Location == location);
                if (contract.IsComplete)
                    return true;
            }
            return false;
        }

        public static void ReplaceContract(this Game game, GameContractLocation location)
        {
            game.DrawContract(location);
        }

        public static GameContract DrawContract(this Game game, GameContractLocation location)
        {
            var deckDict = game.GetContractDecks();
            return DrawContract(game, deckDict, location);
        }

        private static GameContract DrawContract(this Game game, Dictionary<GameContractLocation, List<GameContract>> deckDict, GameContractLocation location)
        {
            if (deckDict.Count == 0)
            {
                ShuffleDeck(game);
                deckDict = game.GetContractDecks();
            }
            var contract = deckDict[GameContractLocation.DrawDeck].First();
            deckDict[GameContractLocation.DrawDeck].Remove(contract);
            contract.Location = location;
            contract.Order = deckDict[location].Count;
            return contract;
        }

        private static void ShuffleDeck(Game game)
        {
            var newDeck = game.Contracts
                .Where(c =>
                    c.Location == GameContractLocation.Draft0 ||
                    c.Location == GameContractLocation.Draft1 ||
                    c.Location == GameContractLocation.Draft2 ||
                    c.Location == GameContractLocation.Draft3 ||
                    c.Location == GameContractLocation.DrawDeck ||
                    c.Location == GameContractLocation.Discard)
                .Shuffle();
            var order = 0;
            foreach(GameContract contract in newDeck)
            {
                contract.Location = GameContractLocation.DrawDeck;
                contract.Order = order++;
            }
        }

        public static List<GameContract> DrawContracts(this Game game)
        {
            var locationOrder = new List<GameContractLocation> { GameContractLocation.Draft0, GameContractLocation.Draft1, GameContractLocation.Draft2, GameContractLocation.Draft3 };
            var deckDict = game.GetContractDecks();
            var drawnContracts = new List<GameContract>();
            foreach ( GameContractLocation location in locationOrder )
            {
                var contract = DrawContract(game, deckDict, location);
                drawnContracts.Add(contract);
            }
            return drawnContracts;
        }

        public static Dictionary<GameContractLocation,List<GameContract>> GetContractDecks( this Game game)
        {
            var result = new Dictionary<GameContractLocation, List<GameContract>> {
                { GameContractLocation.DrawDeck, game.Contracts
                    .Where(c => c.Location == GameContractLocation.DrawDeck)
                    .OrderBy(c => c.Order)
                    .ToList()
                },
                { GameContractLocation.Draft0, game.Contracts
                    .Where(c => c.Location == GameContractLocation.Draft0).OrderByDescending(c => c.Order).ToList()
                },
                { GameContractLocation.Draft1, game.Contracts
                    .Where(c => c.Location == GameContractLocation.Draft1).OrderByDescending(c => c.Order).ToList()
                },
                { GameContractLocation.Draft2, game.Contracts
                    .Where(c => c.Location == GameContractLocation.Draft2).OrderByDescending(c => c.Order).ToList()
                },
                { GameContractLocation.Draft3, game.Contracts
                    .Where(c => c.Location == GameContractLocation.Draft3).OrderByDescending(c => c.Order).ToList()
                },
            };
            return result;
        }

        public static GameContract GetPlayerContractByLocationString(Player player, string contractLocation)
        {
            GameContractLocation location;
            if (Enum.TryParse(contractLocation, out location))
                return null;

            return player.Contracts.FirstOrDefault(c => c.Location == location);
        }

        public static void DiscardContract(Player player, string locationString)
        {
            var contract = GetPlayerContractByLocationString(player, locationString);
            if (contract == null)
                return;

            contract.Location = GameContractLocation.Discard;
            player.Contracts.Remove(contract);
            ReturnAssistantFromContract(player, contract.Location);
        }

        public static void FlipContract(Player player, string locationString, GameContractOrientation orientation = GameContractOrientation.none)
        {
            var contract = GetPlayerContractByLocationString(player, locationString);
            if (contract == null)
                return;

            contract.IsComplete = true;
            contract.Orientation = orientation;
            ReturnAssistantFromContract(player, contract.Location);
        }

        private static void ReturnAssistantFromContract(Player player, GameContractLocation location)
        {
            var assistant = player.Assistants.FirstOrDefault(a => a.Location == ContractToAssistant[location]);
            if (assistant != null)
                AssistantManager.MoveAssistantToOffice(assistant);
        }
    }
}
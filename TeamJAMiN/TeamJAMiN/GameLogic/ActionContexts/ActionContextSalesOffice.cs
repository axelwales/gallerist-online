using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.GameLogic.ComponentManagers;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class SalesOfficeContext : ActionContext
    {
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.ChooseLocation, typeof(ChooseLocation) },
            { GameActionState.SalesOffice, typeof(SalesOffice) },
            { GameActionState.ContractDraft, typeof(ContractDraft) },
            { GameActionState.ContractDraw, typeof(ContractDraw) },
            { GameActionState.ContractToPlayerBoard, typeof(ContractToPlayerBoard) },
            { GameActionState.Pass, typeof(Pass) }
        };

        public SalesOfficeContext(Game game) : base(game, _nameToState) { }
        public SalesOfficeContext(Game game, GameAction action) : base(game, action, _nameToState) { }

    }

    public class SalesOffice : LocationAction
    {
        public SalesOffice()
        {
            Name = GameActionState.SalesOffice;
            location = PlayerLocation.SalesOffice;
            TransitionTo = new HashSet<GameActionState> { GameActionState.ContractDraw, GameActionState.ContractDraft };
        }
    }

    public class ContractDraw : ActionState
    {
        public ContractDraw()
        {
            Name = GameActionState.ContractDraw;
            TransitionTo = new HashSet<GameActionState> { GameActionState.ContractDraft, GameActionState.Pass };
        }

        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            var game = context.Game;
            context.Game.DrawContracts();
            base.DoAction(context);
        }
        //todo add check if the player can take a contract
        public override bool IsValidGameState(ActionContext context)
        {
            return ContractManager.HasAvailableContractLocation(context.Game.CurrentPlayer);
        }
    }

    public class ContractDraft : ActionState
    {
        public ContractDraft()
        {
            Name = GameActionState.ContractDraft;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.ContractToPlayerBoard };
        }
        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            var game = context.Game;
            var location = (GameContractLocation)Enum.Parse(typeof(GameContractLocation), context.Action.StateParams["Location"]);
            var contracts = context.Game.GetContractDecks();
            var contract = contracts[location].First();
            if(context.Game.IsContractLocationEmpty(location))
            {
                context.Game.ReplaceContract(location);
            }
            contract.Location = GameContractLocation.ChooseLocation;
            context.Game.CurrentPlayer.Contracts.Add(contract);
            base.DoAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            GameContractLocation location;
            if (!Enum.TryParse(context.Action.StateParams["Location"], out location))
                return false;

            if (location != GameContractLocation.Draft0 && location != GameContractLocation.Draft1 && location != GameContractLocation.Draft2 && location != GameContractLocation.Draft3)
                return false;

            return ContractManager.HasAvailableContractLocation(context.Game.CurrentPlayer);
        }
    }

    public class ContractToPlayerBoard : ActionState
    {
        public ContractToPlayerBoard()
        {
            Name = GameActionState.ContractToPlayerBoard;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }
        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            var location = (GameContractLocation)Enum.Parse(typeof(GameContractLocation), context.Action.StateParams["Location"]);
            var player = context.Game.CurrentPlayer;
            var contract = player.Contracts.First(c => c.Location == GameContractLocation.ChooseLocation);
            if(player.IsContractLocationEmpty(location))
            {
                //todo give ticket
                var bonusState = ContractManager.PlayerBoardLocationToTicketState[location];
                var bonusExecutable = BonusManager.BonusStateIsExecutable[bonusState];
                context.Game.CurrentTurn.AddPendingAction(new GameAction { Parent = context.Action, State = bonusState, IsExecutable = bonusExecutable });
            }
            else
            {
                //todo discard the current contract and possibly send visitor back to office.
                ContractManager.DiscardContract(player, location);
            }
            contract.Location = location;
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }
        //todo override validate method to check for valid contract location
        //and to check if the location has a contract that can be replaced
        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            GameContractLocation location;
            if (!Enum.TryParse(context.Action.StateParams["Location"], out location))
                return false;

            if (location != GameContractLocation.Vip && location != GameContractLocation.Investor && location != GameContractLocation.Any)
                return false;

            return ContractManager.HasAvailableContractLocation(context.Game.CurrentPlayer);
        }
    }

    public class SellChooseArt : ActionState
    {
        public SellChooseArt()
        {
            Name = GameActionState.SellChooseArt;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.SellChooseVisitor };
        }

        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {

        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            ArtType art;
            if (!Enum.TryParse(context.Action.StateParams["Location"], out art))
                return false;

            if ( context.Game.CurrentPlayer.Contracts.Any(c => c.Art == art && c.IsComplete == false) == false )
                return false;

            return true;
        }
    }

    public class SellChooseVisitor : ActionState
    {
        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {

        }
    }
}
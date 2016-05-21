using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class SalesOfficeContext : ActionContext
    {
        public SalesOfficeContext(Game game)
            : base(game, new Dictionary<GameActionState, Type> {
                { GameActionState.ChooseLocation, typeof(ChooseLocation) },
                { GameActionState.SalesOffice, typeof(SalesOffice) },
                { GameActionState.ContractDraft, typeof(ContractDraft) },
                { GameActionState.ContractDraw, typeof(ContractDraw) },
                { GameActionState.ContractToPlayerBoard, typeof(ContractToPlayerBoard) },
                { GameActionState.Pass, typeof(Pass) }
            })
        { }
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
    }
    public class ContractDraft : ActionState
    {
        public ContractDraft()
        {
            Name = GameActionState.ContractDraft;
            TransitionTo = new HashSet<GameActionState> { GameActionState.ContractToPlayerBoard };
        }
        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            var game = context.Game;
            var location = (GameContractLocation)Enum.Parse(typeof(GameContractLocation), context.Action.Location);
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
        //todo override validate method to check for valid contract location
    }
    public class ContractToPlayerBoard : ActionState
    {
        public ContractToPlayerBoard()
        {
            Name = GameActionState.ContractToPlayerBoard;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }
        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            var location = (GameContractLocation)Enum.Parse(typeof(GameContractLocation), context.Action.Location);
            var player = context.Game.CurrentPlayer;
            var contract = player.Contracts.First(c => c.Location == GameContractLocation.ChooseLocation);
            if(context.Game.IsContractLocationEmpty(location))
            {
                //todo give ticket
            }
            else
            {
                //todo discard the current contract and possibly send visitor back to office.
            }
            contract.Location = location;
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }
        //todo override validate method to check for valid contract location
        //and to check if the location has a contract that can be replaced
    }
    public class SellChooseArt : ActionState
    {
        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {

        }
    }
    public class SellChooseVisitor : ActionState
    {
        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {

        }
    }
}
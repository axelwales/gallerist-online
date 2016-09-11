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
            { GameActionState.TurnStart, typeof(TurnStart) },
            { GameActionState.SalesOffice, typeof(SalesOffice) },
            { GameActionState.ContractDraft, typeof(ContractDraft) },
            { GameActionState.ContractDraw, typeof(ContractDraw) },
            { GameActionState.ContractToPlayerBoard, typeof(ContractToPlayerBoard) },
            { GameActionState.SellChooseVisitor, typeof(SellChooseVisitor) },
            { GameActionState.SellChooseContract, typeof(SellChooseContract) },
            { GameActionState.SellChooseArt, typeof(SellChooseArt) },
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
            TransitionTo = new HashSet<GameActionState> { GameActionState.ContractDraw, GameActionState.ContractDraft, GameActionState.SellChooseArt, GameActionState.SellChooseContract };
        }
    }

    public class ContractDraw : ActionState
    {
        public ContractDraw()
        {
            Name = GameActionState.ContractDraw;
            TransitionTo = new HashSet<GameActionState> { GameActionState.ContractDraft };
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
                ContractManager.DiscardContract(player, context.Action.StateParams["Location"]);
            }
            contract.Location = location;
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }
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
            TransitionTo = new HashSet<GameActionState> { GameActionState.SellChooseVisitor, GameActionState.SellChooseContract };
        }

        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            if ( context.Action.StateParams.ContainsKey("Contract") )
            {
                ArtManager.SellArt(context.Game.CurrentPlayer, context.Action.StateParams["Location"]);

                var visitorAction = new GameAction { State = GameActionState.SellChooseVisitor, Parent = context.Action, IsExecutable = true };
                visitorAction.StateParams["Contract"] = context.Action.StateParams["Contract"];

                bool HasVisitor = context.Game.Visitors.Any(v => v.Location == GameVisitorLocation.Gallery && v.PlayerGallery == context.Game.CurrentPlayer.Color);
                if (HasVisitor == true)
                    visitorAction.IsExecutable = false;
                context.Game.CurrentTurn.AddPendingAction(visitorAction);
            }
            else
            {
                var contractAction = new GameAction() { State = GameActionState.SellChooseContract, Parent = context.Action.Parent };

                contractAction.StateParams["Art"] = context.Action.StateParams["Location"];


                var art = ArtManager.GetPlayerArtByLocationString(context.Game.CurrentPlayer, context.Action.StateParams["Location"]);
                var contracts = context.Game.CurrentPlayer.Contracts.Where(c => c.Art == art.Type);
                if (contracts.Count() == 1)
                {
                    contractAction.StateParams["Location"] = contracts.Single().Location.ToString();
                    contractAction.IsExecutable = true;
                }
                context.Game.CurrentTurn.AddPendingAction(contractAction);
            }
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            int order;
            if (!int.TryParse(context.Action.StateParams["Location"], out order))
                return false;

            var art = context.Game.CurrentPlayer.Art.FirstOrDefault(a => a.Order == order && a.IsSold == false);
            if ( art == null )
                return false;

            if (context.Action.StateParams.ContainsKey("Contract"))
            {
                var contract = ContractManager.GetPlayerContractByLocationString(context.Game.CurrentPlayer, context.Action.StateParams["Contract"]);
                if (contract.Art != art.Type)
                    return false;
            }
            else if (context.Game.CurrentPlayer.Contracts.Any(c => c.IsComplete == false && c.Art == art.Type) == false)
                return false;

            return true;
        }
    }

    public class SellChooseContract : ActionState
    {
        public SellChooseContract()
        {
            Name = GameActionState.SellChooseContract;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.SellChooseVisitor, GameActionState.SellChooseArt };
        }

        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            if (context.Action.StateParams.ContainsKey("Art"))
            {
                ArtManager.SellArt(context.Game.CurrentPlayer, context.Action.StateParams["Art"]);

                var visitorAction = new GameAction { State = GameActionState.SellChooseVisitor, Parent = context.Action, IsExecutable = true };
                visitorAction.StateParams["Contract"] = context.Action.StateParams["Location"];

                bool HasVisitor = context.Game.Visitors.Any(v => v.Location == GameVisitorLocation.Gallery && v.PlayerGallery == context.Game.CurrentPlayer.Color);
                if (HasVisitor == true)
                    visitorAction.IsExecutable = false;
                context.Game.CurrentTurn.AddPendingAction(visitorAction);
            }
            else
            {
                var artAction = new GameAction() { State = GameActionState.SellChooseContract, Parent = context.Action.Parent };

                artAction.StateParams["Contract"] = context.Action.StateParams["Location"];

                var contract = ContractManager.GetPlayerContractByLocationString(context.Game.CurrentPlayer, context.Action.StateParams["Location"]);
                var art = context.Game.CurrentPlayer.Art.Where(a => a.Type == contract.Art);
                if (art.Count() == 1)
                {
                    artAction.StateParams["Location"] = art.Single().Type.ToString();
                    artAction.IsExecutable = true;
                }
                context.Game.CurrentTurn.AddPendingAction(artAction);
            }
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            var contract = ContractManager.GetPlayerContractByLocationString(context.Game.CurrentPlayer, context.Action.StateParams["Location"]);
            if (contract == null || contract.IsComplete == true)
                return false;

            if (context.Action.StateParams.ContainsKey("Art"))
            {
                var art = ArtManager.GetPlayerArtByLocationString(context.Game.CurrentPlayer, context.Action.StateParams["Art"]);
                if (contract.Art != art.Type)
                    return false;
            }
            else if (context.Game.CurrentPlayer.Art.Any(a => a.IsSold == false && a.Type == contract.Art) == false)
                return false;

            return true;
        }
    }

    public class SellChooseVisitor : ActionState
    {
        public SellChooseVisitor()
        {
            Name = GameActionState.SellChooseVisitor;
            RequiredParams = new HashSet<string> { "Contract" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.SellChooseVisitor };
        }

        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            var contractAction = new GameAction { Parent = context.Action, State = GameActionState.SellChooseContractOrientation, IsExecutable = false };
            contractAction.StateParams["Contract"] = context.Action.StateParams["Contract"];
            var visitors = context.Game.Visitors.Where(v => v.Location == GameVisitorLocation.Gallery && v.PlayerGallery == context.Game.CurrentPlayer.Color);
            if (visitors.Count() != 0)
            {
                var location = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.StateParams["Location"]);
                VisitorManager.MoveVisitor(context.Game.CurrentPlayer, location, GameVisitorLocation.Gallery, GameVisitorLocation.Plaza);

                if (location == VisitorTicketType.vip || location == VisitorTicketType.investor)
                {
                    contractAction.StateParams["Location"] = context.Action.StateParams["Location"];
                    contractAction.IsExecutable = true;
                }
            }
            context.Action.Turn.AddPendingAction(contractAction);
            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            bool HasGalleryVisitors = context.Game.Visitors.Any(v => v.Location == GameVisitorLocation.Gallery && v.PlayerGallery == context.Game.CurrentPlayer.Color);
            if (HasGalleryVisitors == false)
                return true;

            VisitorTicketType location;
            if (!Enum.TryParse(context.Action.StateParams["Location"], out location))
                return false;

            bool HasTypeInGallery = context.Game.Visitors.Any(v => v.Location == GameVisitorLocation.Gallery && v.PlayerGallery == context.Game.CurrentPlayer.Color && v.Type == location);
            if (HasTypeInGallery == false)
                return false;

            return true;
        }
    }

    public class SellChooseContractOrientation : ActionState
    {
        public SellChooseContractOrientation()
        {
            Name = GameActionState.SellChooseContractOrientation;
            RequiredParams = new HashSet<string> { "Contract", "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<SalesOfficeContext>(SalesOfficeContext context)
        {
            var orientation = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.StateParams["Location"]);
            if (orientation == VisitorTicketType.investor)
                ContractManager.FlipContract(context.Game.CurrentPlayer, context.Action.StateParams["Contract"], GameContractOrientation.investor);
            else if (orientation == VisitorTicketType.vip)
                ContractManager.FlipContract(context.Game.CurrentPlayer, context.Action.StateParams["Contract"], GameContractOrientation.vip);

            //todo replace below with a pass button or something.
            context.Game.CurrentTurn.AddCompletedAction(context.Action);
            AddPassAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            if (!base.IsValidGameState(context))
                return false;

            VisitorTicketType orientation;
            if (!Enum.TryParse(context.Action.StateParams["Location"], out orientation))
                return false;

            if (orientation == VisitorTicketType.collector)
                return false;

            if (ContractManager.GetPlayerContractByLocationString(context.Game.CurrentPlayer, context.Action.StateParams["Contract"]) == null)
                return false;

            return true;
        }
    }
}
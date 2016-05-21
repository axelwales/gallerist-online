using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class BonusContext : ActionContext
    {
        public bool IsParentState { get; private set; }
        public ActionContext ParentContext { get; private set; }

        public BonusContext(Game game)
            : base(game, new Dictionary<GameActionState, Type> {
                { GameActionState.GetTicketVip, typeof(GetTicketVip) },
                { GameActionState.GetTicketInvestor, typeof(GetTicketInvestor) },
                { GameActionState.GetTicketCollector, typeof(GetTicketCollector) },
                { GameActionState.GetMoney, typeof(GetMoney) },
                { GameActionState.GetInfluence, typeof(GetInfluence) },
                { GameActionState.GetAssistant, typeof(GetAssistant) },
                { GameActionState.ChooseContract, typeof(ChooseContract) },
                { GameActionState.ChooseTicketAny, typeof(ChooseTicketAny) },
                { GameActionState.ChooseTicketAnyTwo, typeof(ChooseTicketAnyTwo) },
                { GameActionState.ChooseTicketCollectorVip, typeof(ChooseTicketCollectorVip) },
                { GameActionState.ChooseTicketCollectorInvestor, typeof(ChooseTicketCollectorInvestor) },
                { GameActionState.ChooseTicketToThrowAway, typeof(ChooseTicketToThrowAway) },
                { GameActionState.ChooseVisitorFromPlaza, typeof(ChooseVisitorFromPlaza) },
                { GameActionState.ChooseVisitorFromPlazaVipInvestor, typeof(ChooseVisitorFromPlazaVipInvestor) },
                { GameActionState.ChooseVisitorFromBag, typeof(ChooseVisitorFromBag) },
                { GameActionState.ChooseArtistFame, typeof(ChooseArtistFame) },
                { GameActionState.GetFame, typeof(GetFame) }
            })
        { }

        protected override GameActionState GetState()
        {
            if (!IsParentState)
                return base.GetState();
            else
                return ParentContext.State;
        }

        protected override void SetState(GameActionState state)
        {
            if (NameToState.ContainsKey(state))
            {
                IsParentState = false;
                base.SetState(state);
            }
            else
            {
                IsParentState = true;
                ParentContext = ActionContextFactory.GetContext(state, Game);
            }
        }

        public override void DoAction(GameAction action)
        {
            if(!IsParentState)
                base.DoAction(action);
        }
    }
    public abstract class BonusAction : ActionState
    {
        //need a solution for transition validation, possibly compile a list of places the bonus action occurs, and use base.IsValidTransition
        //possibly add a checkPendingAction method to validate transition and enforce setting a pending action after a bonus
        //possibly add a set transition method just for bonus actions that caller can use since it probably knows what happens next
        //oh we can use the set transition method for handling executive actions too? except an in memory solution doesn't really work
    }

    public abstract class GetTicket : BonusAction
    {
        public VisitorTicketType Type;
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var game = context.Game;
            if ( game.GetAvailableTicketsByType(Type) == 0 )
            {
                var newAction = new GameAction { State = GameActionState.ChooseTicketToThrowAway, Parent = context.Action, IsExecutable = false };
                game.CurrentTurn.AddPendingAction(newAction);
            }
            game.CurrentPlayer.GetTicket(Type);
            //todo check if ticket stack is newly empty after getting a ticket
            //if stack is newly empty trigger bonuses or add game end condition
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;

            if (game.AvailableVipTickets == 0 && game.AvailableInvestorTickets == 0 && game.AvailableCollectorTickets == 0)
            {
                return false;
            }

            return true;
        }
    }

    public class GetTicketVip : GetTicket
    {
        public GetTicketVip()
        {
            Name = GameActionState.GetTicketVip;
            Type = VisitorTicketType.vip;
            TransitionTo = new HashSet<GameActionState> { };
        }
    }
    public class GetTicketInvestor : GetTicket
    {
        public GetTicketInvestor()
        {
            Name = GameActionState.GetTicketInvestor;
            Type = VisitorTicketType.investor;
            TransitionTo = new HashSet<GameActionState> { };
        }
    }
    public class GetTicketCollector : GetTicket
    {
        public GetTicketCollector()
        {
            Name = GameActionState.GetTicketCollector;
            Type = VisitorTicketType.collector;
            TransitionTo = new HashSet<GameActionState> { };
        }
    }

    public class GetAssistant : BonusAction
    {
        public GetAssistant()
        {
            Name = GameActionState.GetAssistant;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var game = context.Game;
            //todo check if this triggers a bonus
            game.CurrentPlayer.GetNewAssistant();
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            if (game.CurrentPlayer.Assistants.Count >= 10)
            {
                return false;
            }
            return true;
        }
    }

    public class GetMoney : BonusAction
    {
        public GetMoney()
        {
            Name = GameActionState.GetMoney;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var player = context.Game.CurrentPlayer;
            var money = player.GetGalleryVisitorCountByType(VisitorTicketType.investor) + player.GetGalleryVisitorCountByType(VisitorTicketType.collector);
            player.Money += money;
        }
    }
    public class GetInfluence : BonusAction
    {
        public GetInfluence()
        {
            Name = GameActionState.GetInfluence;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var player = context.Game.CurrentPlayer;
            var influence = player.GetGalleryVisitorCountByType(VisitorTicketType.vip) + player.GetGalleryVisitorCountByType(VisitorTicketType.collector);
            player.Influence += influence;
        }
    }

    public class GetFame : BonusAction
    {
        public GetFame()
        {
            Name = GameActionState.GetFame;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var player = context.Game.CurrentPlayer;
            var fame = player.GetGalleryVisitorCountByType(VisitorTicketType.collector);
            var artist = context.Game.GetArtistByLocationString(context.Action.Parent.Location);
            artist.Fame += fame;
        }
    }
    public class ChooseContract : BonusAction
    {
        public ChooseContract()
        {
            Name = GameActionState.ChooseContract;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var newAction = new GameAction { Location = context.Action.Location, State = GameActionState.ContractDraft, IsExecutable = true };
            context.Game.CurrentTurn.AddPendingAction(newAction);
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var newAction = new GameAction { Location = context.Action.Location, State = GameActionState.ContractDraft };
            return newAction.IsValidAction(context.Game);
        }
    }

    public abstract class ChooseTicket : BonusAction
    {
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            var newAction = ActionManager.GetTicketAction(type);
            newAction.Parent = context.Action;
            newAction.IsExecutable = true;
            context.Game.CurrentTurn.AddPendingAction(newAction);
        }
    }

    public class ChooseTicketAny : ChooseTicket
    {
        public ChooseTicketAny()
        {
            Name = GameActionState.ChooseTicketAny;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            var newAction = ActionManager.GetTicketAction(type);
            newAction.Parent = context.Action;
            return newAction.IsValidAction(context.Game);
        }

    }
    public class ChooseTicketAnyTwo : ChooseTicket
    {
        public ChooseTicketAnyTwo()
        {
            Name = GameActionState.ChooseTicketAnyTwo;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<ArtistColonyContext>(ArtistColonyContext context)
        {
            base.DoAction(context);
            if(context.Action.Parent.State != GameActionState.ChooseTicketAnyTwo)
            {
                var newAction = new GameAction { State = GameActionState.ChooseTicketAnyTwo, Parent = context.Action, IsExecutable = false };
                context.Game.CurrentTurn.AddPendingAction(newAction);
            }
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            if(context.Action.Parent.State == GameActionState.ChooseTicketAnyTwo)
            {
                var previousType = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Parent.Location);
                if (previousType == type)
                    return false;
            }
            var newAction = ActionManager.GetTicketAction(type);
            newAction.Parent = context.Action;
            return newAction.IsValidAction(context.Game);
        }
    }
    public class ChooseTicketCollectorVip : ChooseTicket
    {
        public ChooseTicketCollectorVip()
        {
            Name = GameActionState.ChooseTicketCollectorVip;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            if (type == VisitorTicketType.investor)
                return false;
            var newAction = ActionManager.GetTicketAction(type);
            newAction.Parent = context.Action;
            return newAction.IsValidAction(context.Game);
        }
    }
    public class ChooseTicketCollectorInvestor : ChooseTicket
    {
        public ChooseTicketCollectorInvestor()
        {
            Name = GameActionState.ChooseTicketCollectorInvestor;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            if (type == VisitorTicketType.vip)
                return false;
            var newAction = ActionManager.GetTicketAction(type);
            newAction.Parent = context.Action;
            return newAction.IsValidAction(context.Game);
        }
    }
    public class ChooseTicketToThrowAway : BonusAction
    {
        public ChooseTicketToThrowAway()
        {
            Name = GameActionState.ChooseTicketToThrowAway;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<TContext>(TContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            game.RemoveTicketByType(type);
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            if( game.GetAvailableTicketsByType(type) <= 0 )
            {
                return false;
            }
            return true;
        }
    }
    public class ChooseVisitorFromPlaza : BonusAction
    {
        public ChooseVisitorFromPlaza()
        {
            Name = GameActionState.ChooseVisitorFromPlaza;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<TContext>(TContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            game.CurrentPlayer.MoveVisitorPlazaToGallery(type);
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            if (game.GetPlazaVisitorCountByType(type) <= 0)
            {
                return false;
            }
            return true;
        }
    }
    public class ChooseVisitorFromPlazaVipInvestor : ChooseVisitorFromPlaza
    {
        public ChooseVisitorFromPlazaVipInvestor()
        {
            Name = GameActionState.ChooseVisitorFromPlazaVipInvestor;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            if(type == VisitorTicketType.collector)
            {
                return false;
            }
            if (game.GetPlazaVisitorCountByType(type) <= 0)
            {
                return false;
            }
            return true;
        }
    }
    public class ChooseVisitorFromBag : BonusAction
    {
        public ChooseVisitorFromBag()
        {
            Name = GameActionState.ChooseVisitorFromBag;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<TContext>(TContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            game.CurrentPlayer.MoveVisitorBagToGallery(type);
        }
        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.Location);
            if (game.GetBagVisitorCountByType(type) <= 0)
            {
                return false;
            }
            return true;
        }
    }
    public class ChooseArtistFame : BonusAction
    {
        public ChooseArtistFame()
        {
            Name = GameActionState.ChooseArtistFame;
            TransitionTo = new HashSet<GameActionState> { };
        }
        public override void DoAction<TContext>(TContext context)
        {
            var game = context.Game;
            var artist = game.GetArtistByLocationString(context.Action.Location);
            //todo make sure fame caps at 19 (celebrity status)
            artist.Fame += context.Game.CurrentPlayer.GetGalleryVisitorCountByType(VisitorTicketType.collector);
            //todo check if artist just became a celebrity, if so give player celebrity bonus
        }
        public override bool IsValidGameState(ActionContext context)
        {
            if (ArtManager.ValidateArtistLocationString(context.Action) == false)
            {
                return false;
            }
            var game = context.Game;
            var artist = game.GetArtistByLocationString(context.Action.Location);
            if (artist.IsDiscovered == false)
            {
                return false;
            }
            //todo check if artist is a celebrity
            return true;
        }
    }
}
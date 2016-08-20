using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class ExecutiveActionContext : ActionContext
    {
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.UseTicket, typeof(UseTicket) },
            { GameActionState.MoveVisitorStart, typeof(MoveVisitorStart) },
            { GameActionState.MoveVisitorEnd, typeof(MoveVisitorEnd) },
            { GameActionState.UseContractBonus, typeof(UseContractBonus) },
            { GameActionState.Pass, typeof(Pass) }
        };

        public ExecutiveActionContext(Game game) : base(game, _nameToState) { }
        public ExecutiveActionContext(Game game, GameAction action) : base(game, action, _nameToState) { }
    }

    public class UseTicket : ActionState
    {
        public UseTicket()
        {
            Name = GameActionState.UseTicket;
            TransitionTo = new HashSet<GameActionState> { GameActionState.MoveVisitorStart };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            var game = context.Game;
            var newAction = new GameAction { State = GameActionState.MoveVisitorStart, IsExecutable = false, Parent = context.Action };
            game.CurrentTurn.AddPendingAction(newAction);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var type = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.StateParams["Location"]);
            var ticketCount = game.CurrentPlayer.GetPlayerTicketCountByType(type);
            var visitorCount = game.Visitors.Where(v => v.Type == type && (v.Location == GameVisitorLocation.Plaza || v.Location == GameVisitorLocation.Lobby)).Count();
            if(ticketCount <= 0 || visitorCount <= 0)
            {
                return false;
            }
            return true;
        }
    }
    public class MoveVisitorStart : ActionState
    {
        public MoveVisitorStart()
        {
            Name = GameActionState.MoveVisitorStart;
            TransitionTo = new HashSet<GameActionState> { GameActionState.MoveVisitorEnd };
        }

        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            if(VisitorManager.ValidateVisitorLocationString(context.Action.StateParams["Location"]) == false)
            {
                return false;
            }
            var type = VisitorManager.GetVisitorTypeFromLocationString(context.Action.StateParams["Location"]);
            var location = VisitorManager.GetVisitorLocationFromLocationString(context.Action.StateParams["Location"]);
            var color = VisitorManager.GetPlayerColorFromVisitorLocationString(context.Action.StateParams["Location"]);
            if (location != GameVisitorLocation.Lobby && location != GameVisitorLocation.Plaza)
            {
                return false;
            }
            var visitor = game.Visitors.FirstOrDefault(v => v.Type == type && v.Location == location && v.PlayerGallery == color);
            if (visitor == null)
            {
                return false;
            }
            return true;
        }
    }
    public class MoveVisitorEnd : ActionState
    {
        public MoveVisitorEnd()
        {
            Name = GameActionState.MoveVisitorEnd;
            TransitionTo = new HashSet<GameActionState> { GameActionState.UseTicket, GameActionState.Pass };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            var game = context.Game;

            var visitorLocation = VisitorManager.GetVisitorLocationFromLocationString(context.Action.Parent.StateParams["Location"]);
            var visitorPlayerColor = VisitorManager.GetPlayerColorFromVisitorLocationString(context.Action.Parent.StateParams["Location"]);
            var visitorType = VisitorManager.GetVisitorTypeFromLocationString(context.Action.Parent.StateParams["Location"]);
            var visitor = game.Visitors.FirstOrDefault(v => v.Type == visitorType && v.Location == visitorLocation && v.PlayerGallery == visitorPlayerColor);

            var location = VisitorManager.GetVisitorLocationFromLocationString(context.Action.StateParams["Location"]);
            var color = VisitorManager.GetPlayerColorFromVisitorLocationString(context.Action.StateParams["Location"]);

            visitor.UpdateVisitorLocation(location, color);

            base.DoAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            if (VisitorManager.ValidateVisitorLocationString(context.Action.StateParams["Location"]) == false)
            {
                return false;
            }
            var location = VisitorManager.GetVisitorLocationFromLocationString(context.Action.StateParams["Location"]);
            var color = VisitorManager.GetPlayerColorFromVisitorLocationString(context.Action.StateParams["Location"]);
            if (location != GameVisitorLocation.Lobby && location != GameVisitorLocation.Plaza && location != GameVisitorLocation.Gallery)
            {
                return false;
            }
            var visitorLocation = VisitorManager.GetVisitorLocationFromLocationString(context.Action.Parent.StateParams["Location"]);
            var visitorPlayerColor = VisitorManager.GetPlayerColorFromVisitorLocationString(context.Action.Parent.StateParams["Location"]);
            if( location == visitorLocation)
            {
                return false;
            }
            if( location == GameVisitorLocation.Gallery && (visitorLocation != GameVisitorLocation.Lobby || visitorPlayerColor != color))
            {
                return false;
            }
            if( location == GameVisitorLocation.Plaza && visitorLocation != GameVisitorLocation.Lobby)
            {
                return false;
            }
            if( location == GameVisitorLocation.Lobby )
            {
                if(visitorLocation != GameVisitorLocation.Plaza && visitorPlayerColor != color)
                    return false;
            }
            var visitorType = VisitorManager.GetVisitorTypeFromLocationString(context.Action.Parent.StateParams["Location"]);
            if (visitorType == VisitorTicketType.collector && location == GameVisitorLocation.Gallery)
            {
                var collectorCount = context.Game.CurrentPlayer.GetGalleryVisitorCountByType(VisitorTicketType.collector);
                var max = context.Game.CurrentPlayer.Art.Where(a => a.IsSold == true).Count() + 1;
                if(collectorCount == max)
                {
                    return false;
                }
            }           
            return true;
        }
    }
    public class UseContractBonus : ActionState
    {
        public UseContractBonus()
        {
            Name = GameActionState.UseContractBonus;
            TransitionTo = new HashSet<GameActionState> { GameActionState.Pass };
        }

        public override void DoAction<InternationalMarketContext>(InternationalMarketContext context)
        {
            var game = context.Game;
            base.DoAction(context);
        }

        public override bool IsValidGameState(ActionContext context)
        {
            var game = context.Game;
            var currentPlayerLocation = game.CurrentPlayer.GalleristLocation;
            if (currentPlayerLocation == (PlayerLocation)Enum.Parse(typeof(PlayerLocation), Name.ToString()))
            {
                return false;
            }
            return true;
        }
    }
}
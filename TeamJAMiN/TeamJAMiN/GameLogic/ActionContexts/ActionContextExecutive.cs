using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamJAMiN.GalleristComponentEntities;

namespace TeamJAMiN.Controllers.GameLogicHelpers
{
    public class ExecutiveContext : ActionContext
    {
        private static Dictionary<GameActionState, Type> _nameToState = new Dictionary<GameActionState, Type>
        {
            { GameActionState.ChooseTicketToSpend, typeof(ChooseTicketToSpend) },
            { GameActionState.ChooseVisitorToMove, typeof(ChooseVisitorToMove) },
            { GameActionState.MoveVisitorEnd, typeof(MoveVisitorEnd) },
            { GameActionState.UseContractBonus, typeof(UseContractBonus) },
            { GameActionState.PassExecutive, typeof(PassExecutive) },
            { GameActionState.Pass, typeof(Pass) }
        };

        public ExecutiveContext(Game game) : base(game, _nameToState) { }
        public ExecutiveContext(Game game, GameAction action) : base(game, action, _nameToState) { }
    }

    public class PassExecutive : ActionState
    {
        public PassExecutive()
        {
            Name = GameActionState.PassExecutive;
            RequiredParams = new HashSet<string> { };
            TransitionTo = new HashSet<GameActionState> { GameActionState.ChooseVisitorToMove };
        }

    }

    public class ChooseTicketToSpend : ActionState
    {
        public ChooseTicketToSpend()
        {
            Name = GameActionState.ChooseTicketToSpend;
            RequiredParams = new HashSet<string> { "Location" };
            TransitionTo = new HashSet<GameActionState> {  };
        }

        public override void DoAction<ExecutiveContext>(ExecutiveContext context)
        {
            var game = context.Game;
            var ticket = (VisitorTicketType)Enum.Parse(typeof(VisitorTicketType), context.Action.StateParams["Location"]);
            var visitorAction = new GameAction { State = GameActionState.ChooseVisitorToMove, IsExecutable = false, Parent = context.Action };
            visitorAction.StateParams["Visitor"] = context.Action.StateParams["Location"];
            game.CurrentTurn.AddPendingAction(visitorAction);
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
    public class ChooseVisitorToMove : ActionState
    {
        public ChooseVisitorToMove()
        {
            Name = GameActionState.ChooseVisitorToMove;
            RequiredParams = new HashSet<string> { "Location" };
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
            RequiredParams = new HashSet<string> { "Visitor", "Location" };
            TransitionTo = new HashSet<GameActionState> { GameActionState.ChooseTicketToSpend, GameActionState.Pass };
        }

        public override void DoAction<ExecutiveContext>(ExecutiveContext context)
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

        public override void DoAction<ExecutiveContext>(ExecutiveContext context)
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
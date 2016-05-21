using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamJAMiN.DataContexts;
using TeamJAMiN.Controllers.GameControllerHelpers;
using TeamJAMiN.GalleristComponentEntities;
using TeamJAMiN.Models;
using TeamJAMiN.GalleristComponentEntities.Dtos;
using TeamJAMiN.GalleristComponentEntities.Managers;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TeamJAMiN.Controllers.Hubs;
using TeamJAMiN.Controllers.Hubs.HubHelpers;
using TeamJAMiN.Controllers.GameLogicHelpers;
using TeamJAMiN.Models.ComponentViewModels;

namespace TeamJAMiN.Controllers
{
    public class GameController : Controller
    {
        // GET: Game List
        //[AllowAnonymous] We may want to allow anonymous to list games in the future
        [Authorize]
        public ActionResult List()
        {
            var userName = User.Identity.Name;

            using (var identityContext = new ApplicationDbContext())
            {
                var userId = identityContext.Users.First(m => m.UserName == userName).Id;
                using (var galleristContext = new GalleristComponentsDbContext())
                {
                    var allGames = galleristContext.Games.Where(m => !m.IsCompleted && !m.IsDeleted).OrderByDescending(m => m.CreatedTime);
                    var myGames = allGames.Where(m => m.Players.Any(n => n.UserId == userId));

                    var allGamesList = allGames.Select(m => new GameDto
                    {
                        Id = m.Id,
                        Url = "/Game/Play/" + m.Id,
                        Name = m.Name,
                        CurrentNumberOfPlayers = m.Players.Count,
                        MaxNumberOfPlayers = m.MaxNumberOfPlayers,
                        RemainingSlots = m.MaxNumberOfPlayers - m.Players.Count,
                        MaxTurnLength = m.TurnLength,
                        MaxTurnLengthString = m.TurnLength.ToString(),
                        PlayersString = m.Players.Count + " of " + m.MaxNumberOfPlayers,
                        isJoinable = !m.Players.Any(p => p.UserId == userId) && m.Players.Count < m.MaxNumberOfPlayers && !m.IsStarted,
                        CreatedTime = m.CreatedTime
                    }).ToList();

                    var myGamesList = myGames.Select(m => new GameDto
                    {
                        Id = m.Id,
                        Url = "/Game/Play/" + m.Id,
                        Name = m.Name,
                        CurrentNumberOfPlayers = m.Players.Count,
                        MaxNumberOfPlayers = m.MaxNumberOfPlayers,
                        RemainingSlots = m.MaxNumberOfPlayers - m.Players.Count,
                        MaxTurnLength = m.TurnLength,
                        MaxTurnLengthString = m.TurnLength.ToString(),
                        PlayersString = m.Players.Count + " of " + m.MaxNumberOfPlayers,
                        isStartable = m.Players.Any(p => p.UserId == userId && p.IsHost) && !m.IsStarted,
                        isStarted = m.IsStarted,
                        CreatedTime = m.CreatedTime
                    }).ToList();

                    ViewBag.allGames = allGamesList;
                    ViewBag.myGames = myGamesList;
                    return View();
                }
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Create()
        {
            return View(new Game());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Game newGame)
        {
            if (ModelState.IsValid)
            {
                using (var galleristContext = new GalleristComponentsDbContext())
                {
                    newGame.CreatedTime = DateTime.Now;
                    galleristContext.Games.Add(newGame);
                    using (var identityContext = new ApplicationDbContext())
                    {
                        //add me to the game
                        newGame.Players.Add(new Player
                        {
                            UserId = identityContext.Users.First(m => m.UserName == User.Identity.Name).Id,
                            UserName = User.Identity.Name,
                            IsHost = true
                        });
                    }
                    galleristContext.SaveChanges();
                    return Redirect("/Game/List"); //redirect to actual game might be better for demo purposes
                }
            }
            else
            {
                return View(newGame);
            }
        }

        [AuthorizeHostOfGame]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var galleristContext = new GalleristComponentsDbContext())
            {
                var game = galleristContext.Games.FirstOrDefault(m => m.Id == id);

                if(game == null)
                {
                    ViewBag.Message = "This does not appear to be a valid game";
                    ViewBag.Title = "Invalid Game";
                    return View("GameError");
                }

                game.IsDeleted = true;
                
                galleristContext.SaveChanges();
                return Redirect("/Game/List");
            }
        }

        /// <summary>
        /// Takes you to an existing view for a game for which you are a member
        /// </summary>
        /// <returns>Game View</returns>
        [Authorize]
        public ActionResult Play(int id = 0)
        {
            using (var galleristContext = new GalleristComponentsDbContext())
            {
                var gameResponse = GameManager.GetGame(id, galleristContext);

                if (gameResponse.Success && gameResponse.Game.IsStarted)
                {
                    var userName = User.Identity.Name;
                    var playModel = new PlayViewModel(userName, gameResponse.Game);
                    return View(playModel);
                }
                else
                {
                    ViewBag.Message = gameResponse.Message;
                    ViewBag.Title = gameResponse.Title;
                    return View("GameError");
                }
            }
        }

        /// <summary>
        /// Joins an existing game and takes you to the view for that game.
        /// </summary>
        /// <returns>Existing game view or appropriate error</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Join(int id = 0)
        {
            using (var galleristContext = new GalleristComponentsDbContext())
            {
                using (var identityContext = new ApplicationDbContext())
                {
                    var gameResponse = GameManager.GetGame(id, galleristContext);

                    if (gameResponse.Success)
                    {
                        gameResponse.Game.Players.Add(new Player
                        {
                            UserId = identityContext.Users.First(m => m.UserName == User.Identity.Name).Id,
                            UserName = User.Identity.Name
                        });

                        PushHelper singleton = PushHelper.GetPushEngine();
                        singleton.RefreshGameList(gameResponse.Game.Players.Where(m => m.UserName != User.Identity.Name).Select(p => p.UserName).ToList());

                        ViewBag.userName = User.Identity.Name;
                        galleristContext.SaveChanges();
                        return Redirect("/Game/List");
                    }
                    else
                    {
                        ViewBag.Message = gameResponse.Message;
                        ViewBag.Title = gameResponse.Title;
                        return View("GameError");
                    }
                }
            }
        }

        /// <summary>
        /// Starts a game and emails all of the players that their game has started.
        /// </summary>
        /// <param name="id">The id of the game to start</param>
        /// <returns>Existing game view or appropriate error</returns>
        [AuthorizeHostOfGame] //this makes sure the user taking this action is the host of the game
        [HttpPost]
        public ActionResult Start(int id = 0)
        {
            using (var galleristContext = new GalleristComponentsDbContext())
            {
                using (var identityContext = new ApplicationDbContext())
                {
                    //todo: set start time of game to datetime.now
                    var gameResponse = GameManager.GetGame(id, galleristContext);
                    var game = gameResponse.Game;
                    game.StartTime = DateTime.Now;

                    if (game.IsStarted)
                    {
                        return Redirect("~/Game/Play/" + gameResponse.Game.Id);
                    }

                    if (gameResponse.Success)
                    {
                        game.CreateRandomSetup();
                        game.StartGame();
                        galleristContext.SaveChanges();

                        //todo make a helper in email manager for this
                        //todo also use the user and url params for signalr ajax update of game list
                        var gameUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Game/Play/" + game.Id;
                        foreach (var player in game.Players)
                        {
                            var user = identityContext.Users.Single(m => m.Id == player.UserId);
                            if (!user.AllowsEmails || string.IsNullOrWhiteSpace(user.Email))
                                continue;

                            var emailTitle = user.UserName + ", your game has started!"; //todo: get full name of player. We don't have names in the system yet
                            var emailBody = "A game that you are a member of has started. You can play it by visiting The Gallerist Online" +
                                " and viewing your active games or by clicking the following link: " + gameUrl;

                            EmailManager.SendEmail(emailTitle, emailBody, new List<string> { user.Email });
                        }
                        //todo expand module to use signalr for all game list actions
                        PushHelper singleton = PushHelper.GetPushEngine();
                        singleton.UpdateMyGamesList(game.Players.Where(p => p.UserName != User.Identity.Name).Select(p => p.UserName).ToList(), gameUrl, game.Id);
                        return Redirect("~/Game/Play/" + gameResponse.Game.Id);
                    }
                    else
                    {
                        ViewBag.Message = gameResponse.Message;
                        ViewBag.Title = gameResponse.Title;
                        return View("GameError");
                    }
                }
            }
        }

        /// <summary>
        /// Lets a player leave the game and reloads the game with a new turn order
        /// </summary>
        /// <param name="id">The id of the game to leave</param>
        /// <returns>Game list view or appropriate error</returns>
        [AuthorizePlayerInGame]
        [HttpPost]
        public ActionResult Leave(int id = 0)
        {
            using (var galleristContext = new GalleristComponentsDbContext())
            {
                var gameResponse = GameManager.GetGame(id, galleristContext);
                var game = gameResponse.Game;

                if (game.IsStarted)
                {
                    ViewBag.Message = "Cannot leave in progress game";
                    ViewBag.Title = "Error";
                    return View("GameError");
                }

                var playerLeaving = game.Players.First(m => m.UserName == User.Identity.Name);
                galleristContext.Players.Remove(playerLeaving);
                galleristContext.SaveChanges();

                PushHelper singleton = PushHelper.GetPushEngine();
                singleton.RefreshGameList(game.Players.Select(p => p.UserName).ToList());
                return Redirect("~/Game/List/");
            }

        }

        /// <summary>
        /// Skeleton method for taking an action/turn in the game
        /// </summary>
        /// <param name="id">The id of the game to take an action in</param>
        /// <param name="gameAction">The type of action to take along with appropriate values of money/influence spent etc</param>
        /// <returns>Existing game view or appropriate error</returns>
        [ValidateAntiForgeryToken]
        [AuthorizeTurnInGame] //this makes sure it is the current user's turn
        [HttpPost]
        public ActionResult TakeGameAction(int id, GameActionState gameAction, string actionLocation = "")
        {
            using (var galleristContext = new GalleristComponentsDbContext())
            {
                using (var identityContext = new ApplicationDbContext()) //may not need this anymore after adding the authorize helper
                {
                    var gameResponse = GameManager.GetGame(id, galleristContext);
                    var game = gameResponse.Game;
                    
                    //todo todo todo
                    //make sure the player taking an action is the current player
                    //compare current action to game state to make sure a valid action was taken (e.g. player can't move to board spot A from board spot A) [states..]
		            //todo make getting user by username a method (is it one already?)
		            var currentUser = identityContext.Users.FirstOrDefault(m => m.UserName == User.Identity.Name);

		            //todo refactor some of this into a game logic module
		            var player = game.Players.FirstOrDefault(p => p.UserId == currentUser.Id);
		            if (player == null)
		            {
		    	        return View("GameError");
		            }
		            if (player.Id != game.CurrentPlayerId )
		            {
		    	        return View("GameError");
		            }

                    var actionInvoker = new ActionContextInvoker(game);
                    
		            if(!actionInvoker.DoAction(gameAction, actionLocation))
		            {
			            return Redirect("~/Game/Play/" + id);
		            }

                    //check if it is one of the special cases where the action must be confirmed before allowing the next step to proceed (e.g. player must draw cards)
                    //if yes take an intermediate step, still remains current player's turn
                    //if no, continue doing logic things  //determine order of bumped player's actions, can these happen at the end of current player's turn?
                    //check player locations for action taken by current player to see if any players need to be "bumped" to another spot
                    //somewhere in here we need to inject bumped player into turn order, we also need a way to specify that the current "turn" for a bumped player is not a full turn [bumped turn flag or something]
                    //let said player take bumped turn if necessary
                    //again do the updatey
                    //need some signalr stuff so we can show the action to everyone when it is done (intermediate step or not) as well as update money, influence, board, etc.
                    //update money, influence, board, etc.
                    galleristContext.SaveChanges();
                    
                    var nextPlayer = identityContext.Users.First(m => m.Id == game.CurrentPlayer.UserId);
                    if (nextPlayer.AllowsEmails)
                    {
                        var gameUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Game/Play/" + game.Id;
                        var emailTitle = nextPlayer.UserName + ", it is your turn!";
                        var emailBody = "It is your turn in a game you are playing. You can take your turn by visiting The Gallerist Online" +
                            " and viewing your active games or by clicking the following link: " + gameUrl;

                        EmailManager.SendEmail(emailTitle, emailBody, new List<string> { nextPlayer.Email });
                    }
                    PushHelper singleton = PushHelper.GetPushEngine();
                    singleton.RefreshGame(game.Players.Where(p => p.UserName != User.Identity.Name).Select(p => p.UserName).ToList());
                    return Redirect("~/Game/Play/" + id);
                }
            }
        }
    }
}

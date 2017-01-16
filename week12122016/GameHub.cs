using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using GameData;
using System.Collections.ObjectModel;

namespace week12122016
{
    public static class GameSate
    {
        public static List<PlayerData> players = new List<PlayerData>()
    {
        new PlayerData {
            GamerTag = "CHANCEFURLONG",
            Name = "GERARD",
            Password = "CHANCEFURLONG",
            PlayerID = Guid.NewGuid(),
            XP = 2000 },

         new PlayerData {
            GamerTag = "TURDHAMMER",
            Name = "BUDDA",
            Password = "12345",
            PlayerID = Guid.NewGuid(),
            XP = 3000},

         new PlayerData {
            GamerTag = "BIGGESTDICKEST",
            Name = "JAKE",
            Password = "BD",
            PlayerID = Guid.NewGuid(),
            XP = 1000 },

         new PlayerData {
            GamerTag = "THUNDER",
            Name = "TREVOR",
            Password = "SHOCKER",
            PlayerID = Guid.NewGuid(),
            XP = 5000 }
    };

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        public static ObservableCollection<PlayerData> logedInPlayers = new ObservableCollection<PlayerData>();
    }


    public class GameHub : Hub
    {

        public void Hello()
        {
            Clients.All.hello();
        }

        public void ValidatePlayer(string gamertag, string password)
        {
            PlayerData found = GameSate.players.FirstOrDefault(p => p.GamerTag == gamertag && p.Password == password);

            if (!GameSate.logedInPlayers.Contains(found))
            {
                GameSate.logedInPlayers.Add(found);
            }

            if (found != null)
                Clients.Caller.PlayerValidated(found);
            else
            {
                Clients.Caller.Error(new ErrorMess
                { message = "Incorrect GamerTag or Password "});
            }
        }

        public void AllLogedInPlayers(string gamerTag, string password)
        {
            Clients.All.PlayersValidated(GameSate.logedInPlayers);
        }

        public PlayerData getPlayer(string tag, string password)
        {
            PlayerData found = GameSate.players.FirstOrDefault(p => p.GamerTag == tag);

            if (found != null)
                return found;
            else
            {
                Clients.Caller.error(
                    new ErrorMess { message = "Player not found for tag " + tag});
            }
            return found;

        }

        public Joined AllPlayersStartingPositions(float x, float y, string playerID, string ImageName)
        {
            PlayerData found = GameSate.players.FirstOrDefault(p => p.PlayerID.ToString() == playerID);
            Joined hasJoined = new Joined
            {
                playerID = found.PlayerID.ToString(),
                X = x,
                Y = y,
                imageName = ImageName
            };

            Clients.Others.PlayersStartingPositions(new Joined
            {
                playerID = found.PlayerID.ToString(),
                X = x,
                Y = y,
                imageName = ImageName
            });
            return hasJoined;
        }

        public MoveMessage AllPlayersPositions(float x, float y, string playerID)
        {
            PlayerData found = GameSate.players.FirstOrDefault(p => p.PlayerID.ToString() == playerID);
            MoveMessage newPosition = new MoveMessage
            {
                playerID = found.PlayerID.ToString(),
                NewX = x,
                NewY = y,
            };

            Clients.Others.PlayersStartingPositions(new MoveMessage
            {
                playerID = found.PlayerID.ToString(),
                NewX = x,
                NewY = y,
            });

            return newPosition;
        }

        public string SendGroupMessage(string textMessage)
        {
            Clients.All.ShowGroupMessage(textMessage);
            return textMessage;
        }

        public List<CollectableData> GetCollectables(int count, int WorldX, int WorldY)
        {
            List<CollectableData> collectables = new List<CollectableData>();

            for (int i = 0; i < count; i++)
            {
                collectables.Add(new CollectableData
                {
                    ACTION = COLLECTABLE_ACTION.DELIVERED,
                    collectableId = Guid.NewGuid().ToString(),
                    CollectableName = "Collectable " + i.ToString(),
                    collectableValue = GameSate.RandomNumber(20, 100),
                    X = GameSate.RandomNumber(100, WorldX),
                    Y = GameSate.RandomNumber(100, WorldY)
                });
            }
            return collectables;
        }
    }
}
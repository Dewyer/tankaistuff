using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TankServers.Services
{
    public interface IGameServer
    {
        object OpenNewRoom(string name);
        object ConnectToRoom(string roomName);
        List<string> GetRoomNames();
        object SubmitCode(string code, string id);
        object UpdateLobbyData(string id);
    }

    public enum GameState
    {

    }

    public class Player
    {
        public string Code { get; set; }
        public string Id { get; set; }
        public int Score { get; set; }
        public bool Connected { get; set; }
    }

    public class Lobby
    {
        public string Name { get; set; }

        public Player Red { get; set; }
        public Player Blue { get; set; }

    }

    public class GameServer : IGameServer
    {
        public List<Lobby> Lobbies;

        public GameServer()
        {
            Lobbies = new List<Lobby>();
        }


        public object OpenNewRoom(string name)
        {
            if (Lobbies.All(x => x.Name != name))
            {
                var lobby = new Lobby(){Name=name,Red = new Player(){Code = "",Id = Guid.NewGuid().ToString(),Score = 0}, Blue = new Player() { Code = "", Id = Guid.NewGuid().ToString(), Score = 0 } };
                Lobbies.Add(lobby);
            }

            return new { Error = "Error" };
        }

        public object ConnectToRoom(string roomName)
        {
            if (Lobbies.Any(x => x.Name == roomName))
            {
                var lob = Lobbies.First(x => x.Name == roomName);
                if (!lob.Blue.Connected)
                {
                    lob.Blue.Connected = true;
                    return new {Error="None",Data=lob.Blue.Id};
                }
                else if (!lob.Red.Connected)
                {
                    lob.Red.Connected = true;
                    return new { Error = "None", Data = lob.Red.Id };
                }
            }

            return new { error = "Error" };
        }

        public List<string> GetRoomNames()
        {
            return Lobbies.Where(x => !x.Blue.Connected || !x.Red.Connected).Select(x => x.Name).ToList();
        }


        public object UpdateLobbyData(string id)
        {
            var lob = Lobbies.FirstOrDefault(x => x.Blue.Id == id || x.Red.Id == id);
            if (lob != null)
            {
                var other = lob.Blue.Id == id ? lob.Red.Code : lob.Blue.Code;
                return new { Error = "Error", Data = other};
            }
            return new { Error = "Error"};
        }

        public object SubmitResults(string id, string whowon)
        {
            return new { Error = "Error" };
        }

        public object SubmitCode(string code, string id)
        {
            var lob = Lobbies.FirstOrDefault(x => x.Blue.Id == id || x.Red.Id == id);
            if (lob != null)
            {
                if (lob.Blue.Id == id)
                {
                    lob.Blue.Code = code;
                    return new { Error = "None" };
                }
                if (lob.Red.Id == id)
                {
                    lob.Red.Code = code;
                    return new { Error = "None"};
                }
            }

            return new { Error = "Error" };
        }
    }
}

using Dominion.SocketIoServer.Dtos;
using GameModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet;

namespace Dominion.SocketIoServer
{
    public class WebSocketPlayer : IPlayer
    {
        public WebSocketPlayer()
        {

        }
        private SocketIoClient _socket;

        public WebSocketPlayer(SocketIoClient socket)
        {
            Id = Guid.NewGuid().ToString();
            _socket = socket;
        }

        [JsonIgnore]
        public string Id { get; init; }

        public string Name { get; init; }

        [JsonIgnore]
        public PlayerState State { get; init; }

        [JsonIgnore]
        private Game Game { get; set; }

        [JsonIgnore]
        private TaskCompletionSource Task { get; set; }

        public async Task PlayTurn(Game game)
        {
            try
            {
                Game = game;
                Task = new TaskCompletionSource();
                _socket.ListenToAsk("playCard", PlayCard);
                _socket.ListenToAsk("canPlayCard", CanPlayCard);
                _socket.ListenToMessage("buyCards", BuyCard);

                _socket.SendMessage("playTurn", new GameStateDto(Game));

                await Task.Task;

                _socket.OffAsk("playCard", PlayCard);
                _socket.OffAsk("canPlayCard", CanPlayCard);
                _socket.OffMessage("buyCards", BuyCard);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void PlayCard(SocketIOAckEvent ackEvent)
        {
            try
            {
                if (ackEvent.Data.Length < 1 || !ackEvent.Data[0].TryDeserializeObject<PlayCardMessage>(out var playCardMessage))
                {
                    throw new ArgumentException("BadRequest");
                }

                State.PlayCard(Game, playCardMessage, this);

                ackEvent.Callback(new JToken[] { JToken.FromObject(new GameStateDto(Game), JsonSerializer.Create(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })) });
            }
            catch (Exception e)
            {
                _socket.SendMessage("exception", e);
            }
        }

        public void BuyCard(JToken[] data)
        {
            try
            {
                if (data.Length < 1 || !data[0].TryDeserializeObject<BuyMessage>(out var buyMessage))
                {
                    throw new ArgumentException("BadRequest");
                }

                State.BuyCards(Game, buyMessage, this);
                Task.SetResult();
            }
            catch (Exception e)
            {
                _socket.SendMessage("exception", e);
            }
        }

        public void CanPlayCard(SocketIOAckEvent ackEvent)
        {
            try
            {
                if (ackEvent.Data.Length < 1 || !ackEvent.Data[0].TryDeserializeObject<PlayCardMessage>(out var playCardMessage))
                {
                    throw new ArgumentException("BadRequest");
                }

                JToken? resultToken = null;
                try
                {
                    var canPlayCard = State.CanPlayCard(Game, playCardMessage, this);
                    resultToken = JToken.FromObject(canPlayCard);
                }
                catch (Exception)
                {
                    resultToken = JToken.FromObject(false);
                }

                ackEvent.Callback(new JToken[] { resultToken });
            }
            catch (Exception e)
            {
                _socket.SendMessage("exception", e);
            }
        }
    }
}

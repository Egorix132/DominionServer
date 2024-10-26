using Dominion.SocketIoServer.Dtos;
using GameModel;
using GameModel.Infrastructure.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet;

namespace Dominion.SocketIoServer
{
    public class WebSocketPlayer : IPlayer
    {
        private SocketIoClient _socket;

        public WebSocketPlayer(SocketIoClient socket, string name)
        {
            _socket = socket;
            Name = name;
            State = new PlayerState();
        }

        public string Id => _socket.Id.ToString();

        public string Name { get; init; }

        public PlayerState State { get; init; }

        private Game Game { get; set; }

        private TaskCompletionSource TurnTask { get; set; }

        public async Task PlayTurnAsync(Game game)
        {
            try
            {
                Game = game;
                TurnTask = new TaskCompletionSource();
                Subscribe();

                _socket.SendMessage("playTurn", new GameStateDto(Game));

                await TurnTask.Task;

                Unsubscribe();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Subscribe()
        {
            _socket.ListenToAsk("playCard", PlayCard);
            _socket.ListenToAsk("canPlayCard", CanPlayCard);
            _socket.ListenToMessage("buyCards", BuyCard);
        }

        private void Unsubscribe()
        {
            _socket.OffAsk("playCard", PlayCard);
            _socket.OffAsk("canPlayCard", CanPlayCard);
            _socket.OffMessage("buyCards", BuyCard);
        }

        public void PlayCard(SocketIOAckEvent ackEvent)
        {
            PlayCardAsync(ackEvent);
        }

        public async Task PlayCardAsync(SocketIOAckEvent ackEvent)
        {
            try
            {
                if (ackEvent.Data.Length < 1 || !ackEvent.Data[0].TryDeserializeObject<PlayCardMessage>(out var playCardMessage))
                {
                    throw new ArgumentException("BadRequest");
                }

                await State.PlayCard(Game, this, playCardMessage!);

                Game.AddLog(this, playCardMessage!);
                ackEvent.Callback(new JToken[] { JToken.FromObject(new GameStateDto(Game), JsonSerializer.Create(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })) });
            }
            catch (BaseDominionException e)
            {
                Console.WriteLine(e.ToString());
                _socket.SendMessage("exception", e.Message);
                ackEvent.Callback(new JToken[] { JToken.FromObject(new GameStateDto(Game, e.ExceptionType), JsonSerializer.Create(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })) });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _socket.SendMessage("exception", e.Message);
                ackEvent.Callback(new JToken[] { JToken.FromObject(new GameStateDto(Game, ExceptionsEnum.InnerException), JsonSerializer.Create(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })) });
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

                State.BuyCards(Game, buyMessage!, this);
                Game.AddLog(this, buyMessage!);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _socket.SendMessage("exception", e.Message);
            }
            finally
            {
                TurnTask.SetResult();
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
                    var canPlayCard = State.CanPlayCard(Game, playCardMessage!, this);
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
                ackEvent.Callback(Array.Empty<JToken>());
            }
        }

        public async Task<ClarificationResponseMessage> ClarifyPlay(ClarificationRequestMessage request)
        {
            Unsubscribe();

            ClarificationResponseMessage playClarificationResponse = null;
            try
            {
                var response = await _socket.AskAsync(
                    $"clarificatePlay",
                    request);

                if (response.Length < 1 || !response[0].TryDeserializeObject(out playClarificationResponse))
                {
                    throw new ArgumentException("BadRequest");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Subscribe();
            }

            return playClarificationResponse;
        }

        public void GameEnded(GameEndDto gameEndDto)
        {
            Unsubscribe();

            _socket.SendMessage("endGame", gameEndDto);
        }

        public void SendException(Exception e)
        {
            _socket.SendMessage("exception", e.Message);
        }
    }
}

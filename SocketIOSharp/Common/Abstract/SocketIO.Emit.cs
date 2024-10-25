﻿using Newtonsoft.Json.Linq;
using SocketIOSharp.Common.Packet;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocketIOSharp.Common.Abstract
{
    partial class SocketIO<TChildClass>
    {
        private object[] Arguments(params object[] Arguments)
        {
            int ArgumentsCount = Arguments.Length;

            if (Arguments[Arguments.Length - 1] == null)
            {
                ArgumentsCount--;
            }

            object[] Result = new object[ArgumentsCount];

            for (int i = 0; i < ArgumentsCount; i++)
            {
                Result[i] = Arguments[i];
            }

            return Result;
        }

        public TChildClass Emit(JToken Event, Action<JToken[]> Callback = null)
        {
            return Emit(Event, Arguments(Callback));
        }

        public async Task<JToken[]> AskAsync(JToken Event, params object[] args)
        {
            var promise = new TaskCompletionSource<JToken[]>();
            Action<JToken[]> callback = (JToken[] returnData) => {
                promise.SetResult(returnData);
            };

            var argsList = args.ToList();
            argsList.Add(callback);

            Emit(Event, argsList.ToArray());

            return await promise.Task.ConfigureAwait(false);
        }

        public TChildClass Emit(JToken Event, JToken Data, Action<JToken[]> Callback = null)
        {
            return Emit(Event, Arguments(Data, Callback));
        }

        public TChildClass Emit(JToken Event, params object[] Arguments)
        {
            if (Event != null)
            {
                JArray JsonArray = new JArray();
                Action<JToken[]> Callback = null;
                int ArgumentsCount = Arguments.Length;

                if (ArgumentsCount > 0 && Arguments[Arguments.Length - 1] is Action<JToken[]>)
                {
                    ArgumentsCount--;
                    Callback = (Action<JToken[]>)Arguments[Arguments.Length - 1];
                }

                JsonArray.Add(Event);

                for (int i = 0; i < ArgumentsCount; i++)
                {
                    JToken Data;

                    try { Data = JToken.FromObject(Arguments[i], Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto})); }
                    catch { Data = JValue.CreateNull(); }

                    JsonArray.Add(Data);
                }

                Emit(SocketIOPacket.CreateEventPacket(JsonArray, AckManager.CreateAck(Callback)));
            }

            return this as TChildClass;
        }

        internal TChildClass Emit(SocketIOPacket Packet)
        {
            if (Packet != null)
            {
                object Encoded = Packet.Encode();

                if (Packet.IsJson)
                {
                    Emit((string)Encoded);
                }
                else
                {
                    Emit((byte[])Encoded);
                }

                foreach (SocketIOPacket Attachment in Packet.Attachments)
                {
                    Emit(Attachment);
                }
            }

            return this as TChildClass;
        }

        protected abstract TChildClass Emit(string Data);

        protected abstract TChildClass Emit(byte[] RawData);
    }
}

﻿using EmitterSharp;
using SocketIOSharp.Common;
using SocketIOSharp.Server.Client;
using System;

namespace SocketIOSharp.Server
{
    partial class SocketIOServer
    {
        private readonly SocketIOConnectionHandler ConnectionHandlerManager = new SocketIOConnectionHandler();

        public SocketIOServer OnConnection(Action<SocketIOSocket> Callback)
        {
            ConnectionHandlerManager.On(SocketIOEvent.CONNECTION, Callback);

            return this;
        }

        private class SocketIOConnectionHandler : Emitter<SocketIOConnectionHandler, string, SocketIOSocket>
        {

        }
    }
}

﻿using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Channel;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Task;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Node
{
    public class MasterNode
    {
        public IMasterTaskManager TaskManager { get; set; }
        private MasterServer _masterServer { get; set; }
        public MasterNode()
        {
            TaskManager = new MasterTaskManager();

            _masterServer = new MasterServer(GlobalConfig.Config.MasterIp,GlobalConfig.Config.MasterListenPort);
            _masterServer.Connect += ConnectHandler;
            _masterServer.Disconnect += DisconnectHandler;
            _masterServer.ReceiveUpTransmisstion += ReceiveUpTransmisstion;
        }

        public void Start()
        {
            _masterServer.Start();
        }

        public void Stop()
        {
            _masterServer.Connect -= ConnectHandler;
            _masterServer.Disconnect -= DisconnectHandler;
            _masterServer.ReceiveUpTransmisstion -= ReceiveUpTransmisstion;

            _masterServer.Stop();

            TaskManager.Dispose();
        }

        private void DisconnectHandler(string channelId)
        {
            TaskManager.RemoveWorker(channelId);
        }

        private void ConnectHandler(string channelId)
        {
            TaskManager.AddWorker(channelId, "子节点-" + channelId, null);
        }

        private void ReceiveUpTransmisstion(byte[] upMsg)
        {
            TaskManager.RemoveCache(upMsg);
        }
    }
}

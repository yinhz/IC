using Newtonsoft.Json;
using System;

namespace IC.Core
{
    internal interface ICommandProcessor
    {
        RequestCommand ParseCommand(string commandJson);
        bool Processed { get; }
        ResponseCommand InternalProcess(RequestCommand requestCommand, MessageRequest messageRequest);
    }
    /// <summary>
    /// 功能处理接口
    /// </summary>
    internal interface ICommandProcessor<TRequestCommand, TResponseCommand> : ICommandProcessor
        where TRequestCommand : class
        where TResponseCommand : ResponseCommand, new()
    {
        TResponseCommand Process(TRequestCommand requestCommand, MessageRequest messageRequest);
    }

    public abstract class CommandProcessor<TRequestCommand, TResponseCommand> : ICommandProcessor, ICommandProcessor<TRequestCommand, TResponseCommand>
        where TRequestCommand : class
        where TResponseCommand : ResponseCommand, new()
    {
        public CommandProcessor()
        {
        }
        
        public bool Processed { get; private set; }
        
        public ResponseCommand InternalProcess(RequestCommand requestCommand, MessageRequest messageRequest)
        {
            var response = this.Process(requestCommand as TRequestCommand, messageRequest);
            this.Processed = true;
            return response;
        }

        public abstract RequestCommand ParseCommand(string commandJson);

        public abstract TResponseCommand Process(TRequestCommand requestCommand, MessageRequest messageRequest);
    }
}
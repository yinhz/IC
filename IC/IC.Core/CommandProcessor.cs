using Newtonsoft.Json;
using System;

namespace IC.Core
{
    internal interface ICommandProcessor
    {
        bool Processed { get; }
        string InternalProcess(string requestCommandJson);
    }
    /// <summary>
    /// 功能处理接口
    /// </summary>
    internal interface ICommandProcessor<TRequestCommand, TResponseCommand> : ICommandProcessor
        where TRequestCommand : class
        where TResponseCommand : ResponseCommand, new()
    {
        TRequestCommand ParseCommand(string requestCommandJson);
        TResponseCommand InternalProcess(TRequestCommand requestCommand);
        TResponseCommand Process(TRequestCommand requestCommand);
    }

    public abstract class CommandProcessor<TRequestCommand, TResponseCommand> : ICommandProcessor, ICommandProcessor<TRequestCommand, TResponseCommand>
        where TRequestCommand : class
        where TResponseCommand : ResponseCommand, new()
    {
        public CommandProcessor()
        {
        }
        
        public bool Processed { get; private set; }
        
        public virtual TResponseCommand InternalProcess(TRequestCommand requestCommand)
        {
            var response = this.Process(requestCommand as TRequestCommand);
            this.Processed = true;
            return response;
        }

        public virtual string InternalProcess(string requestCommandJson)
        {
            return this.Process(this.ParseCommand(requestCommandJson)).ToJson();
        }

        public abstract TRequestCommand ParseCommand(string requestCommandJson);

        public abstract TResponseCommand Process(TRequestCommand requestCommand);
    }
}
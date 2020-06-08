using Newtonsoft.Json;
using System;

namespace IC.Core
{
    public interface ICommandProcessor
    {
        void Initlize(IConnection connection);
        IConnection Connection { get; }
        bool Processed { get; }
        ResponseCommand InternalProcess(RequestCommand requestCommand);
    }
    /// <summary>
    /// 功能处理接口
    /// </summary>
    public interface ICommandProcessor<TRequestCommand, TResponseCommand>
        where TRequestCommand : class
        where TResponseCommand : IResponseCommand, new()
    {
        void Initlize(IConnection connection);
        IConnection Connection { get; }
        bool Processed { get; }
        TResponseCommand InternalProcess(TRequestCommand requestCommand);
        TResponseCommand Process(TRequestCommand requestCommand);
    }
    
    public abstract class CommandProcessor<TRequestCommand, TResponseCommand> : ICommandProcessor<TRequestCommand, TResponseCommand>
        where TRequestCommand : class
        where TResponseCommand : IResponseCommand, new()
    {
        public CommandProcessor()
        {
            this.ResponseCommand = new TResponseCommand();
        }

        public IConnection Connection { get; private set; }

        public bool Processed { get; private set; }

        public TResponseCommand ResponseCommand { get; private set; }

        public virtual void Initlize(IConnection connection)
        {
            this.Connection = connection;
        }

        public virtual TResponseCommand InternalProcess(TRequestCommand requestCommand)
        {
            this.Processed = true;
            return this.Process(requestCommand);
        }

        public abstract TResponseCommand Process(TRequestCommand requestCommand);
    }
}
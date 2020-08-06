using Newtonsoft.Json;
using System;

namespace IC.Core
{
    public interface ICommandProcessor
    {
        bool Processed { get; }
        string InternalProcess(string requestJson);
    }
    /// <summary>
    /// 功能处理接口
    /// </summary>
    public interface ICommandProcessor<TRequest, TResponse> : ICommandProcessor
        where TRequest : class
        where TResponse : class
    {
        TRequest ParseCommand(string requestJson);
        TResponse InternalProcess(TRequest request);
        TResponse Process(TRequest request);
    }

    public abstract class CommandProcessor<TRequest, TResponse> : ICommandProcessor, ICommandProcessor<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public CommandProcessor()
        {
        }
        
        public bool Processed { get; private set; }
        
        public virtual TResponse InternalProcess(TRequest request)
        {
            var response = this.Process(request as TRequest);
            this.Processed = true;
            return response;
        }

        public virtual string InternalProcess(string requestJson)
        {
            var request = this.ParseCommand(requestJson);
            var response = this.InternalProcess(request);
            if (response == null)
                return string.Empty;
            return response.ToJson();
        }

        public abstract TRequest ParseCommand(string requestJson);

        public abstract TResponse Process(TRequest request);
    }
}
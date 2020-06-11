using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace IC.Core
{
    public interface ICommand
    {
    }

    public interface IRequestCommand : ICommand
    {
        string CommandId { get; }
    }
    
    public abstract class RequestCommand : IRequestCommand
    {
        public abstract string CommandId { get; }
    }

    public interface IResponseCommand : ICommand
    {
        bool IsSuccess { get; set; }
        string ErrorCode { get; set; }
        string ErrorMessage { get; set; }
    }

    public class ResponseCommand : ICommand, IResponseCommand
    {
        public ResponseCommand()
        {

        }
        public virtual bool IsSuccess { get; set; }
        public virtual string ErrorCode { get; set; }
        public virtual string ErrorMessage { get; set; }
    }

    public static class CommandUtils
    {
        public static string ToJson(this ICommand command)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(command);
        }
    }
}
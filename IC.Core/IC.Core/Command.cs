using Newtonsoft.Json;
using System;

namespace IC.Core
{
    public interface ICommand
    {
    }

    public interface IRequestCommand : ICommand
    {
    }

    public abstract class RequestCommand : IRequestCommand
    { }

    public interface IResponseCommand : ICommand
    {
        bool IsSuccess { get; set; }
        string ErrorCode { get; set; }
        string ErrorMessage { get; set; }
    }

    public class ResponseCommand : IResponseCommand
    {
        public ResponseCommand()
        {

        }
        public virtual bool IsSuccess { get; set; }
        public virtual string ErrorCode { get; set; }
        public virtual string ErrorMessage { get; set; }
    }
}
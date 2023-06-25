using System;
using System.ComponentModel;

namespace CommonDomain.DataModels;

public class ServiceResult<T>
{
    public ServiceResult(T payload)
    {
        Status = true;
        Payload = payload;
    }
    public ServiceResult(string message)
    {
        Status = false;
        Message = message;
    }
    public ServiceResult(string message, Exception exception)
    {
        Status = false;
        Message = message;
        ExceptionObject = exception;
    }

    /// <summary>
    /// 此次呼叫是否成功
    /// </summary>
    public bool Status { get; set; } = true;
    public string Message { get; set; } = "";
    public Exception ExceptionObject { get; set; }
    public T Payload { get; set; }

}

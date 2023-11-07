
using KudaGo.Application.Common.Messages;

namespace KudaGo.Application.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandTypeAttribute : Attribute
    {
        public CommandType CommandType { get; }
        public CommandTypeAttribute(CommandType commandType) 
        {
            CommandType = commandType;
        }
    }

}

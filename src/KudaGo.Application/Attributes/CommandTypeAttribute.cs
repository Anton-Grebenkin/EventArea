
using KudaGo.Application.Messages;

namespace KudaGo.Application.Attributes
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

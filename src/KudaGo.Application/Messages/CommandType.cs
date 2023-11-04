using System.ComponentModel;

namespace KudaGo.Application.Messages
{
    public enum CommandType
    {
        [Description("/start")]
        Start = 0,
        [Description("/city")]
        City = 1,
        [Description("/categories")]
        Categories = 2
    }

    public static class CommandTypeHelper
    {
        private static readonly Type CommandType = typeof(CommandType);
        private static readonly Type DescriptionAttributeType = typeof(DescriptionAttribute);
        public static string GetCommandString(this CommandType commandType) 
        {
            var field = CommandType.GetField(commandType.ToString());
            var attribute = Attribute.GetCustomAttribute(field, DescriptionAttributeType) as DescriptionAttribute;
            return attribute != null ? attribute.Description : commandType.ToString();
        }
    }


}

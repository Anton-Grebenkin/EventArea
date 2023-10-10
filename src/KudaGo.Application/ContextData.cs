

namespace KudaGo.Application
{
    public class ContextData
    {
        public bool UserExists { get; set; }
        public IEnumerable<string> RequiredCommandsForUser { get; set; }
    }
}

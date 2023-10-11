using KudaGo.Application.Abstractions;
using KudaGo.Application.Data;

namespace KudaGo.Application.Services
{
    //public class ContextService : IContextService<ContextData>
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly IConfigurationRepository _configurationRepository;
    //    public ContextService(IUserRepository userRepository, IConfigurationRepository configurationRepository) 
    //    {
    //        _userRepository = userRepository;
    //        _configurationRepository = configurationRepository;
    //    } 
    //    public async Task<ContextData> RefreshContextDataAsync(long chatId)
    //    {
    //        var result = new ContextData();

    //        var requiredCommandsForNewUser = (await _configurationRepository.GetConfigurationAsync()).RequiredCommandsForNewUser;

    //        result.UserExists = await _userRepository.UserExistsAsync(chatId);
    //        if (result.UserExists)
    //        {
    //            var user = await _userRepository.GetUserAsync(chatId);

    //            if (user.)
    //        }
    //        else
    //        {
    //            result.RequiredCommandsForUser = (await _configurationRepository.GetConfigurationAsync()).RequiredCommandsForNewUser;
    //        }
    //    }
    //}
}

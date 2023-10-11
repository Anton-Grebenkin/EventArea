using KudaGo.Application.Abstractions;
using KudaGo.Application.ApiClient;
using KudaGo.Application.ApiClient.Dto;
using KudaGo.Application.Attributes;
using KudaGo.Application.Data;
using KudaGo.Application.Extensions;
using KudaGo.Application.Messages;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KudaGo.Application.CallbackHandlers
{
    //[CallbackType(CallbackType.CitySelection)]
    //public class CitySelectionCallbackHandler : ICallbackHandler
    //{
    //    private readonly IUserRepository _userRepository;
    //    private readonly ITelegramBotClient _botClient;
    //    private readonly IRedirectService _redirectService;
    //    private readonly IKudaGoApiClient _kudaGoApiClient;
    //    public CitySelectionCallbackHandler(
    //        IUserRepository userRepository, 
    //        ITelegramBotClient botClient, 
    //        IRedirectService redirectService,
    //        IKudaGoApiClient kudaGoApiClient
    //        ) 
    //    {
    //        _userRepository = userRepository;
    //        _botClient = botClient;
    //        _redirectService = redirectService;
    //        _kudaGoApiClient = kudaGoApiClient;
    //    }
    //    public async Task HandleAsync(CallbackQuery callbackQuery, CallbackData callbackData, CancellationToken cancellationToken)
    //    {
    //        var user = await _userRepository.GetUserAsync(callbackQuery.Message.Chat.Id);

    //        user.City = callbackData.Data;

    //        var ci = await _kudaGoApiClient.GetCityAsync(callbackData.Data);

    //        await _userRepository.UpdateUserAsync(user);

    //        await _botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);

    //        if (callbackData.NextCommand != null)
    //        {
    //            await _redirectService.RedirectAsync(callbackData.NextCommand, callbackQuery.Message, cancellationToken);
    //        }
    //    }
    //}
}

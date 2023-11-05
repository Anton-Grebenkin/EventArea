﻿using KudaGo.Application.Services;


namespace KudaGo.TelegramBot.Workers
{
    public class UpdateEventsWorker : BackgroundService
    {
        private readonly ILogger<UpdateEventsWorker> _logger;
        private readonly IUpdateEventsService _updateEventsService;
        public UpdateEventsWorker(IUpdateEventsService updateEventsService, ILogger<UpdateEventsWorker> logger) 
        {
            _logger = logger;
            _updateEventsService = updateEventsService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _updateEventsService.UpdateEvents();

                    await Task.Delay(3600000, stoppingToken);
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex.Message);
                }
            }
        }
    }
}
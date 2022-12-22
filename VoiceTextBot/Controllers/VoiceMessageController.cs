using Telegram.Bot;
using Telegram.Bot.Types;
using VoiceTextBot.Configuration;
using VoiceTextBot.Services;

namespace VoiceTextBot.Controllers
{
    public class VoiceMessageController
    {
        private readonly AppSettings _appSettings;
        private readonly ITelegramBotClient _telegramClient;
        private readonly IFileHandler _audioFileHandler;
        private readonly IStorage _memoryStorage;
        public VoiceMessageController(AppSettings appSettings, ITelegramBotClient telegramBotClient, IFileHandler audioFileHandler, IStorage memoryStorage)
		{
			_appSettings = appSettings;
			_telegramClient = telegramBotClient;
			_audioFileHandler = audioFileHandler;
			_memoryStorage = memoryStorage;
		}

		public async Task Handle(Message message, CancellationToken ct)
        {
            var fileId = message.Voice?.FileId;
            if (fileId == null)
                return;

            await _audioFileHandler.Download(fileId, ct);

            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Голосовое сообщение загружено", cancellationToken: ct);

            string userLanguageCode = _memoryStorage.GetSession(message.Chat.Id).LanguageCode;
            _audioFileHandler.Process(userLanguageCode);
            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Голосовое сообщение конвертировано в формат .WAV", cancellationToken: ct);
        }
    }
}

using GTicket_User_API.Models.Email;
using GTicket_User_API.Services.Interface;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GTicket_User_API.Services.Implement
{
    public class JsonFileEmailLogStore : IEmailLogStore
    {
        private const int MaxEntries = 500;
        private static readonly SemaphoreSlim FileLock = new(1, 1);
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly string _filePath;

        public JsonFileEmailLogStore(ILogger<JsonFileEmailLogStore> logger)
        {
            var folder = Path.Combine(Path.GetTempPath(), "GTicket");
            Directory.CreateDirectory(folder);
            _filePath = Path.Combine(folder, "email-logs.json");
            logger.LogInformation("Email log file: {Path}", _filePath);
        }

        public async Task AddAsync(EmailLogModel log, CancellationToken cancellationToken)
        {
            await FileLock.WaitAsync(cancellationToken);
            try
            {
                var logs = await ReadAsync(cancellationToken);
                logs.Add(log);

                if (logs.Count > MaxEntries)
                    logs = logs.Skip(logs.Count - MaxEntries).ToList();

                await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(logs, JsonOptions), cancellationToken);
            }
            finally
            {
                FileLock.Release();
            }
        }

        public async Task<IReadOnlyList<EmailLogModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            await FileLock.WaitAsync(cancellationToken);
            try
            {
                var logs = await ReadAsync(cancellationToken);
                return logs.OrderByDescending(l => l.SentAtUtc).ToList();
            }
            finally
            {
                FileLock.Release();
            }
        }

        private async Task<List<EmailLogModel>> ReadAsync(CancellationToken cancellationToken)
        {
            if (!File.Exists(_filePath)) return [];

            var json = await File.ReadAllTextAsync(_filePath, cancellationToken);
            if (string.IsNullOrWhiteSpace(json)) return [];

            try
            {
                return JsonSerializer.Deserialize<List<EmailLogModel>>(json, JsonOptions) ?? [];
            }
            catch (JsonException)
            {
                return [];   // file corrupt aana puthusa start
            }
        }
    }
}

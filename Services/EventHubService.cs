using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EduSync.API.Interfaces;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EduSync.API.Services
{
    public class EventHubService : IEventHubService
    {
        private readonly EventHubProducerClient _producerClient;

        public EventHubService(IConfiguration configuration)
        {
            // These will be read from Key Vault via App Service Configuration
            var connectionString = configuration["EventHubConnectionString"];
            var eventHubName = configuration["EventHubName"];

            _producerClient = new EventHubProducerClient(connectionString, eventHubName);
        }

        public async Task SendQuizAttemptAsync(object quizAttemptData)
        {
            using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();

            var jsonData = JsonSerializer.Serialize(quizAttemptData);
            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(jsonData))))
            {
                // Log an error if the message is too large for the batch
                throw new Exception($"Event data for quiz attempt is too large for a single batch.");
            }

            await _producerClient.SendAsync(eventBatch);
        }
    }
}
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace autocare_api.Services
{
    public class SnsNotificationService
    {
        private readonly IAmazonSimpleNotificationService _sns;
        private readonly IConfiguration _config;

        public SnsNotificationService(
            IAmazonSimpleNotificationService sns,
            IConfiguration config)
        {
            _sns = sns;
            _config = config;
        }

        public async Task SubscribeEmailAsync(string email)
        {
            var topicArn = _config["AWS:SnsTopicArn"];

            var request = new SubscribeRequest
            {
                TopicArn = topicArn,
                Protocol = "email",
                Endpoint = email
            };

            await _sns.SubscribeAsync(request);
        }
    }
}

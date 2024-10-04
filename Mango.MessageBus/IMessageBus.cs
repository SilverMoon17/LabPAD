namespace Mango.MessageBus;

public interface IMessageBus
{
    Task PublishMessage(BaseMessage msg, string topicName);
}
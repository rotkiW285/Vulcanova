using AutoMapper;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages;

public class MessageBoxMapperProfile : Profile
{
    public MessageBoxMapperProfile()
    {
        CreateMap<MessageBoxPayload, MessageBox>();

        CreateMap<MessagePayload, Message>();
    }
}
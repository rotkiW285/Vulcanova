using AutoMapper;

namespace Vulcanova.Features.Messages.Compose;

public class AddressBookEntryMapperProfile : Profile
{
    public AddressBookEntryMapperProfile()
    {
        CreateMap<Uonet.Api.MessageBox.AddressBookEntry, AddressBookEntry>()
            .ForMember(dest => dest.Id, o => o.MapFrom(src => src.GlobalKey));
    }
}
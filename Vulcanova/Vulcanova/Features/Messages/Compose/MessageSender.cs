using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vulcanova.Core.Uonet;
using Vulcanova.Features.Auth;
using Vulcanova.Uonet.Api.Common.Models;
using Vulcanova.Uonet.Api.MessageBox;

namespace Vulcanova.Features.Messages.Compose;

public class MessageSender : IMessageSender
{
    private readonly IApiClientFactory _apiClientFactory;
    private readonly IAccountRepository _accountRepository;

    public MessageSender(IAccountRepository accountRepository, IApiClientFactory apiClientFactory)
    {
        _accountRepository = accountRepository;
        _apiClientFactory = apiClientFactory;
    }

    public async Task SendMessageAsync(int accountId, AddressBookEntry recipient, string subject, string message)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);

        var apiClient = await _apiClientFactory.GetAuthenticatedAsync(account);

        var threadKey = Guid.NewGuid();

        var request = new SendMessageRequest
        {
            Id = Guid.NewGuid(),
            GlobalKey = threadKey,
            Partition = account.Partition,
            ThreadKey = threadKey,
            Subject = subject,
            Content = message,
            Status = 1,
            Owner = recipient.MessageBoxId,
            DateSent = DateTimeInfo.FromDateTime(DateTime.Now),
            DateRead = null,
            Sender = new SendMessageRequestCorrespondent
            {
                Id = "0",
                Partition = account.Partition,
                Owner = recipient.MessageBoxId,
                GlobalKey = recipient.MessageBoxId,
                HasRead = 0
            },
            Receiver = new List<SendMessageRequestCorrespondent>
            {
                new SendMessageRequestCorrespondent
                {
                    Id = $"{recipient.MessageBoxId}-{recipient.Id}",
                    Partition = account.Partition,
                    Owner = recipient.MessageBoxId,
                    GlobalKey = recipient.Id,
                    HasRead = 0
                }
            },
            Attachments = new List<Attachment>()
        };

        await apiClient.PostAsync(SendMessageRequest.ApiEndpoint, request);
    }
}
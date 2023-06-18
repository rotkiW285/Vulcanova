using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Vulcanova.Features.Settings.HttpTrafficLogger;

public class LoggingHttpMessageHandler : DelegatingHandler
{
    public ReadOnlyObservableCollection<HttpTrafficLogEntry> Requests { get; private set; }

    private readonly ObservableCollection<HttpTrafficLogEntry> _requests = new();

    private static readonly SemaphoreSlim Semaphore = new (1, 1);

    public static LoggingHttpMessageHandler Instance
    {
        get
        {
            Semaphore.Wait();
            try
            {
                return _instance ??= new();
            }
            finally
            {
                Semaphore.Release();
            }
        }
    } 

    private static LoggingHttpMessageHandler _instance;

    private LoggingHttpMessageHandler()
    {
        InnerHandler = new HttpClientHandler();

        Requests = new ReadOnlyObservableCollection<HttpTrafficLogEntry>(_requests);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var body = request.Content != null
            ? await request.Content.ReadAsStringAsync()
            : null;

        var entry = new HttpTrafficLogEntry
        {
            RequestMessage = request,
            RequestBody = body
        };

        await Semaphore.WaitAsync(cancellationToken);

        _requests.Add(entry);

        Semaphore.Release();

        var response = await base.SendAsync(request, cancellationToken);

        body = response.Content != null
            ? await response.Content.ReadAsStringAsync()
            : null;

        entry.ResponseMessage = response;
        entry.ResponseBody = body;

        return response;
    }
}

public class HttpTrafficLogEntry : ReactiveObject
{
    public HttpRequestMessage RequestMessage { get; init; }
    public string RequestBody { get; init; }
    
    [Reactive]
    public HttpResponseMessage ResponseMessage { get; set; }
    
    [Reactive]
    public string ResponseBody { get; set; }

    public DateTime DateTime { get; } = DateTime.Now;

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine(RequestMessage.RequestUri.ToString());
        builder.AppendLine("Request:");

        foreach (var (key, values) in RequestMessage.Headers)
        {
            builder.Append(key);
            builder.Append(": ");
            builder.AppendJoin(", ", values);
            builder.Append(Environment.NewLine);
        }

        builder.AppendLine(RequestBody);
        
        builder.AppendLine("Response:");

        if (ResponseMessage == null)
        {
            builder.AppendLine("Response missing");
        }
        else
        {
            foreach (var (key, values) in ResponseMessage.Headers)
            {
                builder.Append(key);
                builder.Append(": ");
                builder.AppendJoin(", ", values);
                builder.Append(Environment.NewLine);
            }

            builder.AppendLine(ResponseBody);
        }

        return builder.ToString();
    }
}
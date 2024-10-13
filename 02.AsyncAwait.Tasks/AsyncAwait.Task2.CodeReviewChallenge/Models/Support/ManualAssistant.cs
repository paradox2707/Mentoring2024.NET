using AsyncAwait.Task2.CodeReviewChallenge.Models.Support;
using CloudServices.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System;

public class ManualAssistant : IAssistant
{
    private readonly ISupportService _supportService;

    public ManualAssistant(ISupportService supportService)
    {
        _supportService = supportService ?? throw new ArgumentNullException(nameof(supportService));
    }

    public async Task<string> RequestAssistanceAsync(string requestInfo)
    {
        try
        {
            await _supportService.RegisterSupportRequestAsync(requestInfo).ConfigureAwait(false);

            await Task.Delay(5000); // this is just to be sure that the request is registered

            return await _supportService.GetSupportInfoAsync(requestInfo).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            return $"Failed to register assistance request. Please try later. {ex.Message}";
        }
    }
}
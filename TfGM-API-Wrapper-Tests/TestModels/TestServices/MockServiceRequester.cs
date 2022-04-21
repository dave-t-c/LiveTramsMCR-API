using System.Collections.Generic;
using TfGM_API_Wrapper.Models.Services;

namespace TfGM_API_Wrapper_Tests.TestModels.TestServices;

public class MockServiceRequester : IRequester
{
    private const string ValidApiResponsePath = "../../../Resources/ExampleApiResponse.json";

    // ReSharper disable once EmptyConstructor
    public MockServiceRequester()
    {
    }

    public List<UnformattedServices> RequestServices(List<int> ids)
    {
        if (ids.Contains(701))
            return new List<UnformattedServices>
            {
                ImportServicesResponse.ImportUnformattedServices(ValidApiResponsePath) ?? new UnformattedServices()
            };

        return new List<UnformattedServices>();
    }
}
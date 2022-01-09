using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static System.AppDomain;

namespace TfGM_API_Wrapper.Models.Resources
{
    /// <summary>
    /// Loads in the Stop data from the resources folder.
    /// </summary>
    public class StopLoader
    {
        private readonly ResourcesConfig _resourcesConfig;

        /// <summary>
        /// Create a new StopLoader Object, which can import the required Stops.
        /// </summary>
        /// <param name="resourcesConfig"></param>
        public StopLoader(ResourcesConfig resourcesConfig)
        {
            LoaderHelper loaderHelper = new LoaderHelper();

            _resourcesConfig = resourcesConfig ?? throw new ArgumentNullException(nameof(resourcesConfig));

            _resourcesConfig.StopResourcePath = loaderHelper.CheckFileRequirements(resourcesConfig.StopResourcePath,
                nameof(resourcesConfig.StopResourcePath));
            
            _resourcesConfig.StationNamesToTlarefsPath = loaderHelper.CheckFileRequirements(resourcesConfig.StationNamesToTlarefsPath,
                nameof(resourcesConfig.StationNamesToTlarefsPath));
            
            _resourcesConfig.TlarefsToIdsPath = loaderHelper.CheckFileRequirements(resourcesConfig.TlarefsToIdsPath,
                nameof(resourcesConfig.TlarefsToIdsPath));
        }

        /// <summary>
        /// Imports the Stops from the Stops resources file.
        /// The results from this are then returned with a GET to '/api/stops'.
        /// </summary>
        public List<Stop> ImportStops()
        {
            using var reader = new StreamReader(_resourcesConfig.StopResourcePath);
            var jsonString = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<Stop>>(jsonString);
        }
        
        
    }
}
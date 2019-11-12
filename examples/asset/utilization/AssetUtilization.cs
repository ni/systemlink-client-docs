using NationalInstruments.SystemLink.Clients.AssetManagement;

namespace NationalInstruments.SystemLink.Clients.Examples.Asset.Utilization
{
    /// <summary>
    /// Example for the SystemLink Asset Management Client API that
    /// tracks utilization for assets present in the system.
    /// 
    /// Note: As a prerequisite for this example, ensure SystemLink Client is installed.
    /// </summary>
    class AssetUtilization
    {
        static void Main(string[] args)
        {
            /*
             * The Asset Utilization Store enables asset utilization tracking. 
             * It is created using a factory.
             */
            var utilizationStoreFactory = new AssetUtilizationStoreFactory();
            using (var assetUtilizationStore = utilizationStoreFactory.CreateAssetUtilizationStore())
            {
                /*
                 * Configuring the API to send heartbeats for ongoing sessions at predefined intervals.
                 * By default, heartbeats are automatically sent every 5 minutes.
                 */
                var utilizationConfiguration = StartUtilizationConfiguration.CreateDefault();

                /*
                 * Start a utilization session for all assets in the system.
                 * For starting a utilization session, the following information is nedeed:
                 * - The utilization configuration
                 * - [Optional] asset name(s): a single asset name or a collection of asset names. if this parameter is omitted, all assets are tracked.
                 * - Utilization category: The reason for using the asset(s), such as for tests, configuration, maintenance, or custom purposes.
                 * - User name: The name of the operator using the asset.
                 * - Application name: The program which is using the asset(s).
                 * 
                 * The StartUtilization information returns an IStartedUtilization instance which ends utilization when it is disposed.
                 * Any asset operation needs to be placed inside the using block.
                 */
                using (var utilizationSession = assetUtilizationStore.StartUtilization(utilizationConfiguration, "DUT Testing", "john.doe", "Custom Testing App"))
                {
                    // asset operations go here
                }
            }
        }
    }
}
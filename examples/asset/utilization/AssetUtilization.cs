using NationalInstruments.SystemLink.Clients.AssetManagement;

namespace NationalInstruments.SystemLink.Clients.Examples.Asset.Utilization
{
    /// <summary>
    /// Example for the SystemLink Asset Management client API that
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
            var assetUtilizationStore = utilizationStoreFactory.CreateAssetUtilizationStore();

            /*
             * Tracking Asset Utilization can be configured to send heartbeats
             * for ongoing utilization sessions, at predefined intervals. 
             * By default, heartbeats are automatically sent every 5 minutes.
             */
            var utilizationConfiguration = StartUtilizationConfiguration.CreateDefault();

            /*
             * Start an utilization session for all assets in the system.
             * For starting an utilization session, the following information is nedeed:
             * - the utilization configuration,
             * - [optional] asset names: a single asset name or a collection of asset names. if this parameter is ommited, all assets are tracked.
             * - utilization category: what category does this utilization have? (test, configuration, maintenance, custom)
             * - user name: of the operator using the asset
             * - application name
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
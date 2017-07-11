namespace Tinifier.Core.Services.BackendDevs
{
    public interface IBackendDevsConnector
    {
        /// <summary>
        /// Send statistic to http://backend-devs.com/
        /// </summary>
        /// <param name="domainName">user domainName</param>
        void SendStatistic(string domainName);
    }
}

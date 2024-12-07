using WebProtocol;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckMaintenanceReq req = new CheckMaintenanceReq() { ServerName = "dev", Version = "1.0.0" };

            var res = HttpAgent.RequestLobbyAsync<CheckMaintenanceRes>("http://127.0.0.1:5001/Gate", req);
            var result = res.Result;

            AllServerUrlReq req2 = new AllServerUrlReq();
            var res2 = HttpAgent.RequestLobbyAsync<AllServerUrlRes>("http://127.0.0.1:8000/Gate", req2);
            var result2 = res2.Result;

            Console.WriteLine($"result: {result.Result}, {result.ProtocolId}, {result.ServerUrl}");
            Console.WriteLine($"result2: {result2.Result}, {result2.ProtocolId}, {result2.RankServerUrl}, {result2.GuildServerUrl}");

            Console.ReadKey();
        }
    }
}

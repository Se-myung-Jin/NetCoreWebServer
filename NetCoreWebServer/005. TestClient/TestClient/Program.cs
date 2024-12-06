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

            Console.WriteLine($"result: {result.Result}, {result.ProtocolId}, {result.ServerUrl}");

            Console.ReadKey();
        }
    }
}

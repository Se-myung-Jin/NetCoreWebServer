using WebProtocol;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CheckReq req = new CheckReq() { Name = "Maintenance", Number = 5001 };

            var res = HttpAgent.RequestLobbyAsync<CheckRes>("http://127.0.0.1:5001/Gate", req);
            var result = res.Result;

            Console.WriteLine($"result: {result.IsOk}, {result.ProtocolId}");

            Console.ReadKey();
        }
    }
}

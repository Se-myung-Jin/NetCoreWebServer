using System.Xml.Linq;
using WebServerCore;

namespace MaintenanceServer
{
    public static class MaintenanceManager
    {
        public static bool IsMaintenance(string serverName, string version)
        {
            var maintenance = TableMaintenance.Find(serverName, version);
            if (maintenance == null) return false;

            var now = DateTime.UtcNow;
            if (maintenance.Enable == false || maintenance.StartTime >= now || now >= maintenance.EndTime)
            {
                return false;
            }

            return true;
        }
    }
}

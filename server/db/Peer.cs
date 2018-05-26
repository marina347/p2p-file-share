using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server.BazaF {
    public class Peer:ICrud {
        int applicationId;
        IPAddress ipAddress;
        DateTime lastVisit;

        public int ApplicationId {
            get {
                return applicationId;
            }

            set {
                applicationId = value;
            }
        }

        public IPAddress IpAddress {
            get {
                return ipAddress;
            }

            set {
                ipAddress = value;
            }
        }

        public DateTime LastVisit {
            get {
                return lastVisit;
            }

            set {
                lastVisit = value;
            }
        }

        public Peer(SqlDataReader dr) {
            if (dr != null) {
                applicationId = int.Parse(dr["application_id"].ToString()); 
                ipAddress = IPAddress.Parse(dr["ip_address"].ToString());  
                lastVisit = DateTime.Parse(dr["last_visit"].ToString());      
            }
        }

        public Peer() {

        }

        public int Add() {
            string sqlQuery = "";
            string sqlFormattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sqlQuery = "INSERT INTO peer (ip_address,last_visit) OUTPUT INSERTED.application_id VALUES  ('" + ipAddress + "','" + sqlFormattedDate + "')";
            return (int)Database.Instance.GetValue(sqlQuery);
        }

        public int Update(int appIdInput) {
            string sqlQuery = "";
            string sqlFormattedDate = lastVisit.ToString("yyyy-MM-dd HH:mm:ss");
            sqlQuery = "UPDATE peer SET ip_address = '" + ipAddress + "', last_visit = '" + sqlFormattedDate + "' WHERE application_id = " + appIdInput;
            return Database.Instance.ExecuteQuery(sqlQuery);
            
        }

        public int Delete() {
            return 0;
        }

        public static List<Peer> GetPeers() {
            List<Peer> list = new List<Peer>();
            string sqlQuery = "SELECT * FROM peer";
            SqlDataReader dr = Database.Instance.GetDataReader(sqlQuery);
            while (dr.Read()) {
                Peer peer = new Peer(dr);
                list.Add(peer);
            }
            dr.Close(); 
            return list;
        }
        }
}

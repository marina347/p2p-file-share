using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.BazaF {
    public class Owner {
        int applicationId;
        int fileId;

		public static void DeleteAll() {
			String sqlQuery = "DELETE FROM owner";
			Database.Instance.ExecuteQuery(sqlQuery);
		}

		public int ApplicationId {
            get {
                return applicationId;
            }

            set {
                applicationId = value;
            }
        }

        public int FileId {
            get {
                return fileId;
            }

            set {
                fileId = value;
            }
        }
        public Owner(SqlDataReader dr) {
            if (dr != null) {
                applicationId = int.Parse(dr["application_id"].ToString());
                fileId = int.Parse(dr["file_id"].ToString());

            }
        }
        public Owner() {

        }
        public int Add() {
            string sqlQuery = "";
            sqlQuery = "INSERT INTO owner (application_id,file_id) VALUES (" + applicationId + ", " + fileId + ")";
            return Database.Instance.ExecuteQuery(sqlQuery);
        }
        public int Update(int fileIdInput,int appIdInput) {
            string sqlQuery = "";
            sqlQuery = "UPDATE owner SET application_id = '" + applicationId + "', file_id = '" + fileId + "' WHERE file_id = " + fileIdInput + " AND application_id="+appIdInput;
            return Database.Instance.ExecuteQuery(sqlQuery);

        }
        public int Delete() {
            return 0;
        }
		
		public static int Delete(int applicationId, int fileId) {
			string sqlQuery = "DELETE FROM owner WHERE file_id="+fileId+" AND application_id="+applicationId;
			return Database.Instance.ExecuteQuery(sqlQuery);
		}

		public static List<Owner> GetOwners() {
            List<Owner> list = new List<Owner>();
            string sqlQuery = "SELECT * FROM owner";
            SqlDataReader dr = Database.Instance.GetDataReader(sqlQuery);
            while (dr.Read()) {
                Owner owner = new Owner(dr);
                list.Add(owner);
            }
            dr.Close(); 
            return list;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace server.BazaF {
    public class Database {
		const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|Dijeljenje_datoteka.mdf;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=10";
        private static Database instance; 
                                      
        private SqlConnection connection; 
        public static Database Instance 
            {
            get {
                if (instance == null) 
                    { instance = new Database(); }
                return instance;
            }
        }

        public SqlConnection Connection {
			get {
				return connection;
			}
			private set {
					connection = value;
			}
		}

        private Database() {
			string executable = System.Reflection.Assembly.GetEntryAssembly().Location;
			string path = (System.IO.Path.GetDirectoryName(executable));
			AppDomain.CurrentDomain.SetData("DataDirectory", path);
			Connection = new SqlConnection(connectionString);
            Connection.Open();
		}
		
        public SqlDataReader GetDataReader(string sqlQuery) {
            SqlCommand command = new SqlCommand(sqlQuery, Connection);
            return command.ExecuteReader();
        }
        public object GetValue(string sqlQuery) {
            SqlCommand command = new SqlCommand(sqlQuery, Connection);
            return command.ExecuteScalar();
        }

        public int ExecuteQuery(string sqlQuery) {
            SqlCommand command = new SqlCommand(sqlQuery, Connection);
            return command.ExecuteNonQuery();
        }
		
		public int ExecuteQuery(SqlCommand sqlCommand) {
			sqlCommand.Connection = this.connection;
			return sqlCommand.ExecuteNonQuery();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonResources;

namespace server.BazaF {
    public class ChunkHashModel {
        int fileId;
        byte[] hash;
        int hashNumber;

        public int FileId {
            get {
                return fileId;
            }

            set {
                fileId = value;
            }
        }

        public byte[] Hash {
            get {
                return hash;
            }

            set {
                hash = value;
            }
        }

        public int HashNumber {
            get {
                return hashNumber;
            }

            set {
                hashNumber = value;
            }
        }
        public ChunkHashModel(SqlDataReader dr) {
            if (dr != null) {
                fileId = int.Parse(dr["file_id"].ToString());
                hash = (byte[])dr["hash"];
                hashNumber = int.Parse(dr["chunk_id"].ToString());

            }
        }
        public ChunkHashModel() {

        }
        public int Add() {
            
            string sqlQuery = "";
			SqlCommand cmd = new SqlCommand();
			cmd.Parameters.Add("@binaryHash", SqlDbType.VarBinary, 8000).Value = hash;
			sqlQuery = "INSERT INTO chunk (file_id,hash,chunk_id) VALUES (" + fileId + ", @binaryHash," + hashNumber + ")";
			cmd.CommandText = sqlQuery;

			return Database.Instance.ExecuteQuery(cmd);
        }

		private static string GetHashString(byte[] hash) {
			StringBuilder Sb = new StringBuilder();
			foreach (Byte b in hash)
				Sb.Append(b.ToString("x2"));
			return Sb.ToString();
		}

		public static void Add(ChunkHash[] hashes, int fileId) {
			
			string sqlQuery = "INSERT INTO chunk (file_id,hash,chunk_id) VALUES ";
			SqlCommand cmd = new SqlCommand();
			int i = 0;
			
			for (i=0; i<hashes.Length; i++) {
				sqlQuery += "(";
				sqlQuery += fileId+",";
				sqlQuery += "@binaryHash"+i+",";
				sqlQuery += hashes[i].ChunkId;
				sqlQuery += "),";
				cmd.Parameters.Add("@binaryHash"+i, SqlDbType.VarBinary, 8000).Value = hashes[i].Hash;
				if (i!=0 && i != hashes.Length-1 && i % 999 == 0) {
					sqlQuery = sqlQuery.TrimEnd(',');
					cmd.CommandText = sqlQuery;
					Database.Instance.ExecuteQuery(cmd);
					cmd.Parameters.Clear();
					sqlQuery = "INSERT INTO chunk (file_id,hash,chunk_id) VALUES ";
				}
			}
			sqlQuery = sqlQuery.TrimEnd(',');
			cmd.CommandText = sqlQuery;
			Database.Instance.ExecuteQuery(cmd);
		}
		
		public int Update(int fileIdInput) {
            string sqlQuery = "";
            sqlQuery = "UPDATE chunk SET file_id = '" + this.fileId + "', hash = '" + hash + "', chunk_id = '" + hashNumber + "' WHERE file_id = " + fileIdInput;
            return Database.Instance.ExecuteQuery(sqlQuery);

        }
		
        public int Delete() {
            return 0;
        }

		public static void DeleteAll() {
			String sqlQuery = "DELETE FROM chunk";
			Database.Instance.ExecuteQuery(sqlQuery);
		}

		public static List<ChunkHashModel> GetChunksByFileId(int fileId) {
			List<ChunkHashModel> list = new List<ChunkHashModel>();
			string sqlQuery = "SELECT * FROM chunk WHERE chunk.file_id=" + fileId;
			SqlDataReader dr = Database.Instance.GetDataReader(sqlQuery);
			while (dr.Read()) {
				ChunkHashModel chunk = new ChunkHashModel(dr);
				list.Add(chunk);
			}
			dr.Close(); 
			return list;
		}

		public static byte[] GetChunkByChunkIdAndFileId(int chunkId, int fileId) {
			List<ChunkHashModel> list = new List<ChunkHashModel>();
			string sqlQuery = "SELECT * FROM chunk WHERE chunk.file_id=" + fileId+ " AND chunk.chunk_id=" + chunkId;
			SqlDataReader dr = Database.Instance.GetDataReader(sqlQuery); 
			if(dr.Read()) {
				ChunkHashModel chunk = new ChunkHashModel(dr);
                dr.Close();
                return chunk.hash;
			}
			else
            {
                dr.Close();
                return null;
			}
		}
	}
}

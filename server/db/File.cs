using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace server.BazaF {
    public class File {
        int fileId;
        string fileName;
        string fileExtension;
        uint fileSize;
        int applicationId;

        public int FileId {
            get {
                return fileId;
            }

            set {
                fileId = value;
            }
        }

        public string FileName {
            get {
                return fileName;
            }

            set {
                fileName = value;
            }
        }

        public string FileExtension {
            get {
                return fileExtension;
            }

            set {
                fileExtension = value;
            }
        }

        public uint FileSize {
            get {
                return fileSize;
            }

            set {
                fileSize = value;
            }
        }

        public int ApplicationId {
            get {
                return applicationId;
            }

            set {
                applicationId = value;
            }
        }
        public File(SqlDataReader dr) {
            if (dr != null) {
                applicationId = int.Parse(dr["application_id"].ToString());
                fileId = int.Parse(dr["file_id"].ToString());
                fileName = (dr["name"].ToString());
                fileExtension= (dr["extension"].ToString());
                fileSize= uint.Parse(dr["size"].ToString());

            }
        }
        public File() {

        }
        public int Add() {
            
            string sqlQuery = "";
            sqlQuery = "INSERT INTO _file (name,size,extension,application_id) OUTPUT INSERTED.file_id VALUES ('" + fileName + "', " + fileSize + ", '" + fileExtension + "', "+applicationId+")";
			fileId = (int) Database.Instance.GetValue(sqlQuery);
			return this.fileId;
        }
        public int Update(int fileIdInput) {
            string sqlQuery = "";
            sqlQuery = "UPDATE _file SET file_id = '" + fileId + "', name = '" + fileName + "', size = '" + fileSize + "', extension = '" +fileExtension + "', application_id = '"+applicationId +"' WHERE file_id = " + fileIdInput;
            return Database.Instance.ExecuteQuery(sqlQuery);

        }
        public int Delete() {         
            return 0;
        }
		public static void DeleteAll() {
			String sqlQuery = "DELETE FROM _file";
			Database.Instance.ExecuteQuery(sqlQuery);
		}

		public static List<File> GetFiles() {
			List<File> list = new List<File>();
			string sqlQuery = "SELECT * FROM _file";
			SqlDataReader dr = Database.Instance.GetDataReader(sqlQuery);
			while (dr.Read()) {
				File file = new File(dr);
				list.Add(file);
			}
			dr.Close(); 
			return list;
		}

		public static List<File> GetFiles(string namePart) {
			List<File> list = new List<File>();
			string sqlQuery = "SELECT * FROM _file WHERE LOWER(_file.name) LIKE '%" + namePart.ToLower()+ "%'";
			SqlDataReader dr = Database.Instance.GetDataReader(sqlQuery);
			while (dr.Read()) {
				File file = new File(dr);
				list.Add(file);
			}
			dr.Close(); 
			return list;
		}
		public static List<Peer> GetFileOwners(int fileId) {
			List<Peer> list = new List<Peer>();
			string sqlQuery = "SELECT * FROM peer p WHERE p.application_id IN (SELECT application_id FROM owner WHERE file_id = "+fileId+")";
			SqlDataReader dr = Database.Instance.GetDataReader(sqlQuery);
			while (dr.Read()) {
				Peer peer = new Peer(dr);
				list.Add(peer);
			}
			dr.Close(); 
			return list;
		}

		public static List<int> GetFileOwnersId(int fileId) {
			return GetFileOwners(fileId).Select(o => o.ApplicationId).ToList();
		}

		public static bool Exists(string name, string ext, uint size) {

			string sqlQuery = "SELECT 1 FROM _file WHERE _file.name = '" + name+ "' AND _file.extension='" + ext+"' AND size = "+size;
			return Database.Instance.GetValue(sqlQuery)!= null;
		}
    }
}

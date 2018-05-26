using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CommonResources;

namespace client {
	class NewFileTransferManager {
		
		public static NewFileTransferManager nftm;
		private void CreateDescriptorFile(FileDescription fd, ChunkHash[] hashes) {
			FileStream fileStream = new FileStream(".\\descriptors\\descriptor_" + fd.FileName +"."+ fd.FileExtension+".desc", FileMode.Create, FileAccess.Write);
			
			byte[] data = new byte[Sizes.GetChunksNumber(fd.FileSize)];
			for (int i = 0; i < data.Length; i++)
				data[i] = 0;
			fileStream.Write(data, 0, data.Length);
			
			foreach(ChunkHash h in hashes) {
				fileStream.Write(h.Hash, 0, h.Hash.Length);
			}
			fileStream.Flush();
			fileStream.Close();
		}

		private void CreateEmptyFile(FileDescription fd, string pathFile) {
			FileStream fileStream = new FileStream(pathFile + "\\" + fd.FileName +"."+ fd.FileExtension, FileMode.Create, FileAccess.Write);
			fileStream.Write(new byte[fd.FileSize], 0, (int)fd.FileSize);
			fileStream.Flush();
			fileStream.Close();
		}

		public void NewDownloadFile(FileDescription fd, ChunkHash[] hashes) {
			
			CreateDescriptorFile(fd, hashes);
			CreateEmptyFile(fd, fd.Path);

		}
        
		public void NewUploadFile(FileDescription fd, string existingFilePath) {

        }
    }
}

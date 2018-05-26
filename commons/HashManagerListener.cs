using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources {
	public interface HashManagerListener {
		void HashDownloadFinished(ChunkHash[] hashes, int fileId, int senderId);
		void RequestResend(int chunkHashId, int senderId, int fileId);
	}
}

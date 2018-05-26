using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources {
	public enum MessageType {
		c2s_ConnectRequest,
		c2s_HolePunchRequest,
		c2s_ClientAvailabilityRequest,
		c2s_FileClientListRequest,

		s2c_ConnectResponse,
		s2c_TryHolePunch, 
		s2c_TryLocalHolePunch,
		s2c_FileClientListResponse,
		s2c_NewClientEndPoint,
		s2c_RequestedClientAvailable,

		c2c_GotHolePunch, 
		c2c_HolePunchSuceeded,
		c2c_DisconnectedClientEndPoint, 
		c2c_CloseConnection,

		ft_FileRegisterResponse,
		ft_FileRegisterFinished,
		ft_FileRegistrationRequest,
		ft_FileInitTransferRequest,
		ft_FileInitTransferResponse,
		ft_FileHashTransferRequest,
		ft_ResendChunkHashRequest,
		ft_ChunkHash,
		ft_FileStateRequest,
		ft_FileStateResponse,
		ft_SendChunksRequest,
		ft_ChunkPartData,
		ft_ChunkPartResendRequest,
		c2s_FileSearchByNameRequest,
		s2c_FileSearchByNameResponse,

		c2s_ApplicationRegisterResponse,
		c2s_FileDeleted,
	}
}

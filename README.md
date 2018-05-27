# p2p-file-share
 P2P File sharing app with central server

## General information about app<br>
-> Inspiration - Bittorent and Napster <br>
-> Three projects (Client, Server, Common resources) <br>
-> .NET C# <br> 
-> Transact-SQL <br>
-> UDP protocol <br>
-> UDP Hole punching to resolve problem of NAT <br>
-> Central server that has only information about files <br>
-> P2P clients that sends and receives chunks <br>
-> Binary file for tracking application state <br>
-> Testing - Server with public IP address; Clients on different geographic locations; TeamViewer v12 for managing clients in real time <br>
           
## Application architecture
Client and server projects use common dll (third project called CommonResources). Their arhitectures have minor differences so next picture shows Client architecture.
This arhitecture allows sending and receiving files at same time with more than one peer for every file. Receiving is sequential, chunks are reached in array.
There are three layers: network layer, layer for transfering files and application layer. Network layer makes a connection, sends and receives messages.
Layer for transfering files is central for application, it offers all structure and logic for transfering files. Classes for downloading and uploading files are separated in order
to implement the layer granularly. Application layer consists of forms or we can say it is user interface. It also allows to register own file in system, start downloading and has
classes for working with main binary file.

![arch](https://github.com/marina347/p2p-file-share/blob/master/p2p_slike/arhitektura.png?raw=true)


## How to use application

**Client's file transfers** <br>
When user starts this application, he can see all his transfers including uploading and dowloading. Every file has name, extension, size and completion percentage.
If user wants to registrate his own file in system, he needs to click on button Browse and choose file.

![view](https://github.com/marina347/p2p-file-share/blob/master/p2p_slike/pregled.png?raw=true)


**Search** <br>
If user wants to download specific file, he can choose Search from menu. When he finds it, he needs to click twice on it in order to choose where he wants to save it. User can go back to File transfers now to follow up the downloading process.

![search](https://github.com/marina347/p2p-file-share/blob/master/p2p_slike/pretraga.png?raw=true)


**Downloading process** <br>
Progress can be seen through the percentage that increases as user receives chunks of files. If he closes an application while downloading file, it will continue downloading it next time application is started.

![download](https://github.com/marina347/p2p-file-share/blob/master/p2p_slike/skidanje.png?raw=true)

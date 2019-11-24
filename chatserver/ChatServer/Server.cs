using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer {
	class Server {
		private TcpListener ear;
		private TcpClient cli_sock;
		private List<ClientManager> connectedClients;
		private int nbConnectedClients;

		public Server() {
			connectedClients = new List<ClientManager>();
			nbConnectedClients = 0;

			byte[] local_IP = { 127, 0, 0, 1 };
			ear = new TcpListener(new IPAddress(local_IP), 8080);

			// start listening for client requests
			ear.Start();

			while(true) {
				Console.WriteLine("Waiting for connection...");
				cli_sock = ear.AcceptTcpClient(); // blocking

				Console.WriteLine("Connection detected!!!");

				// start a new client manager
				connectedClients.Add(new ClientManager(cli_sock));
				Thread threadClientManager = new Thread(new ThreadStart(connectedClients[nbConnectedClients++].run));

				threadClientManager.Start();
			}

			Console.ReadLine();
		}
	}
}

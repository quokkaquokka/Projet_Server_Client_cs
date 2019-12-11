using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChatServer {
	class Server {
		private TcpListener ear;
		private TcpClient cli_sock;
		private List<ClientManager> connectedClients;
		private int nbConnectedClients;

		private List<User> users;
		private List<string> topics;

		public Server() {
			users = new List<User>();
			topics = new List<string>();
			Restore();
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

				// create a new client manager for this client
				connectedClients.Add(new ClientManager(cli_sock, this));

				// start the new client manager
				Thread threadClientManager = new Thread(new ThreadStart(connectedClients[nbConnectedClients++].run));
				threadClientManager.Start();
			}

			Console.ReadLine();
		}

		// serialize the data of the users and the topics in a file
		public void Backup() {
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream("data/users.txt", FileMode.Create, FileAccess.Write);
			formatter.Serialize(stream, users);
			stream.Close();

			stream = new FileStream("data/topics.txt", FileMode.Create, FileAccess.Write);
			formatter.Serialize(stream, topics);
			stream.Close();
		}

		// deserialize the data of the users and the topics from a file
		public void Restore() {
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream("data/users.txt", FileMode.Open, FileAccess.Read);
			users = (List<User>) formatter.Deserialize(stream);
			stream.Close();

			stream = new FileStream("data/topics.txt", FileMode.Open, FileAccess.Read);
			topics = (List<string>)formatter.Deserialize(stream);
			stream.Close();
		}

		public List<ClientManager> ConnectedClients {
			get { return connectedClients; }
		}

		public List<User> Users {
			get { return users; }
		}

		public List<string> Topics {
			get { return topics; }
		}
	}
}

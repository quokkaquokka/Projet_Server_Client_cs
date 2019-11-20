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

		public Server() {
			byte[] local_IP = { 127, 0, 0, 1 };
			ear = new TcpListener(new IPAddress(local_IP), 8080);

			// start listening for client requests
			ear.Start();

			Console.WriteLine("Waiting for connection...");
			cli_sock = ear.AcceptTcpClient(); // blocking

			Console.WriteLine("Connection detected!!!");

			Thread.Sleep(2000);

			StreamWriter sOut = new StreamWriter(cli_sock.GetStream());
			sOut.AutoFlush = true;
			sOut.WriteLine("Coucou c'est moi, le serveur. Tu es bien connecté!");

			// now talk with the client
			Console.ReadLine();
		}
	}
}

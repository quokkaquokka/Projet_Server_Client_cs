using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ChatServer {
	class ClientManager {
		private TcpClient clientSocket;
		private StreamWriter sOut;
		private StreamReader sIn;
		private string msg;

		public ClientManager(TcpClient clientSocket) {
			this.clientSocket = clientSocket;
			sOut = new StreamWriter(clientSocket.GetStream());
			sOut.AutoFlush = true;

			sIn = new StreamReader(clientSocket.GetStream());

			msg = "";
		}

		public void run() {
			// send a message to the new client
			sOut.WriteLine("Welcome {name of the subject}, from {city of the subject}");

			msg = sIn.ReadLine();
			while(msg != "/q") {
				Console.WriteLine(msg);
				// apply the action depending on the message
				msg = sIn.ReadLine();
			}
		}
	}
}

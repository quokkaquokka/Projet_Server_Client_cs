using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient {
	class Receiver {
		private Client client;
		private StreamReader sIn;
		private string msg;
		private string[] msgSplit;

		public Receiver(TcpClient serverSocket, Client client) {
			sIn = new StreamReader(serverSocket.GetStream());
			this.client = client;
		}

		public void run() {
			bool quit = false;
			while(!quit) {
				// wait for messages to display
				msg = sIn.ReadLine();
				msgSplit = msg.Split(':');

				// if the message begin with "Server: " it is an instruction to execute
				if(msgSplit[0] == "Server")
					switch(msgSplit[1]) {
						case " bye":
							quit = true;
							break;
						default:
							Console.WriteLine(msg);
							break;
					}
				// otherwise display the message
				else
					Console.WriteLine(msg);
			}

			Console.WriteLine("Press Enter to quit");
			client.Quit();
		}
	}
}

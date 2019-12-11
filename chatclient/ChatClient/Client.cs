using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatClient {
	class Client {
		private TcpClient sock;
		private StreamWriter sOut;
		private string msg, username;
		private bool quit;

		public Client() {
			byte[] local_IP = { 127, 0, 0, 1 };

			// try to connect to the server
			Console.WriteLine("Connecting...");
			bool connectionOk = false;
			while(!connectionOk) {
				try {
					sock = new TcpClient("localhost", 8080);
					connectionOk = true;
				}
				catch(Exception e) { connectionOk = false; }
			}
			Console.WriteLine("Connected!!!");

			// create and start the receiver to run alongside the application
			Receiver receiver = new Receiver(sock, this);
			Thread threadReceiver = new Thread(new ThreadStart(receiver.run));
			threadReceiver.Start();

			// send messages to the server
			sOut = new StreamWriter(sock.GetStream());
			sOut.AutoFlush = true;

			// the client just have to type anything
			// the server will interpret or reject it
			msg = "";
			quit = false;
			while(!quit) {
				msg = Console.ReadLine();
				sOut.WriteLine(msg);
			}
		}

		public void Quit() {
			quit = true;
		}
	}
}

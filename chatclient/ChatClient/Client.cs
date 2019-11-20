using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient {
	class Client {
		private TcpClient sock;
		public Client() {
			byte[] local_IP = { 127, 0, 0, 1 };

			Console.WriteLine("Connecting...");
			sock = new TcpClient("localhost", 8080);
			Console.WriteLine("Connected!!!");
			// now talk with the server if connected

			StreamReader sIn = new StreamReader(sock.GetStream());
			string msg = sIn.ReadLine();
			Console.WriteLine(msg);

			Console.ReadLine();
		}
	}
}

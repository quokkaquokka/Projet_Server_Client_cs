using System;
using System.Collections.Generic;
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
			// now talk with the server if connected

			Console.WriteLine("Connected!!!");
			Console.ReadLine();
		}
	}
}

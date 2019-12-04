using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient {
	class Receiver {
		private StreamReader sIn;
		private string msg;

		public Receiver(TcpClient serverSocket) {
			sIn = new StreamReader(serverSocket.GetStream());
		}

		public void run() {
			// wait for messages to display
			while(true) {
				msg = sIn.ReadLine();
				Console.WriteLine(msg);
			}
		}
	}
}

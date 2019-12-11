using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ChatServer {
	// Class launched in a thread for each client who log in
	class ClientManager {
		private Server server;
		private TcpClient clientSocket;
		private StreamWriter sOut;
		private StreamReader sIn;
		private string msg, username;

		public ClientManager(TcpClient clientSocket, Server server) {
			this.server = server;
			this.clientSocket = clientSocket;

			// Initialisation of the StreamWriter and the StreamReader
			sOut = new StreamWriter(clientSocket.GetStream());
			sOut.AutoFlush = true;
			sIn = new StreamReader(clientSocket.GetStream());

			msg = "";
			username = "";
		}

		public void run() {
			// Connection menu
			bool next = false;
			bool quit = false;
			while(!next) {
				clearClientScreen();
				sOut.WriteLine("What do you want to do?\n" +
					"1: Log In\n"     +
					"2: Register\n\n" +
					"0: Quit\n\n"     +
					"Your choice: "   );
				msg = sIn.ReadLine();

				switch(msg) {
					case "0":
						clearClientScreen();
						sOut.WriteLine("Server: bye");
						next = true;
						quit = true;
						break;
					case "1":
						next = Login();
						break;
					case "2":
						next = Register();
						break;
					default:
						break;
				}
			}

			// Main menu
			while(!quit) {
				clearClientScreen();
				sOut.WriteLine("Welcome " + username + "!\n" +
					"What do you want to do?\n" +
					"1: Join a topic\n" +
					"2: Create a new topic\n" +
					"3: Send private messages\n\n" +
					"0: Quit\n\n" +
					"Your choice: ");
				msg = sIn.ReadLine();

				switch(msg) {
					case "0":
						clearClientScreen();
						sOut.WriteLine("Server: bye");
						quit = true;
						break;
					case "1":
						JoinTopic();
						break;
					case "2":
						CreateTopic();
						break;
					case "3":
						SendPrivateMessages();
						break;
					default:
						break;
				}
			}

			sOut.WriteLine("Server: bye");
		}

		// Ask the user for credentials and check if they are valid
		public bool Login() {
			clearClientScreen();
			sOut.WriteLine("Log In\n----------");

			string password, username;

			sOut.WriteLine("Type your username (or /q to go back):");
			username = sIn.ReadLine();

			if(username != "/q" && username != "") {
				sOut.WriteLine("Type your password:");
				password = sIn.ReadLine();
				if(password != "/q" && password != "") {
					// check credentials
					foreach(User user in server.Users) {
						if(user.Username == username && user.Password == password) {
							this.username = username;
							return true;
						}
					}
				}
			}

			return false;
		}

		// Ask the user for credentials and add a new user in the list of the server
		public bool Register() {
			clearClientScreen();
			sOut.WriteLine("Register\n----------");

			string password, username;

			sOut.WriteLine("Type your username (or /q to go back):");
			username = sIn.ReadLine();

			if(username != "/q" && username != "") {
				sOut.WriteLine("Type your password:");
				password = sIn.ReadLine();
				if(password != "/q" && password != "") {
					// add the user in the list of the server
					server.Users.Add(new User(username, password));
					server.Backup();

					return true;
				}
			}

			return false;
		}

		public void JoinTopic() {
			clearClientScreen();
		}

		public void CreateTopic() {
			clearClientScreen();

			bool next = false;
			while(!next) {
				sOut.WriteLine("Type the name of the topic you want to create (or /q to go back):");
				msg = sIn.ReadLine();

				next = true;
				if(msg != "/q" && msg != "") {
					// check if the topic already exists
					foreach(string topic in server.Topics) {
						if(msg == topic) {
							next = false;
							clearClientScreen();
							sOut.WriteLine("Sorry, this topic already exists.");
						}
					}

					// if the name doesn't already exits
					if(next) {
						server.Topics.Add(msg);
						server.Backup();
					}
				}
			}
		}

		public void SendPrivateMessages() {
			clearClientScreen();
		}

		// Check if the message is a number between 0 and the max value
		public bool MessageOk(string msg, int max) {
			string cntString;

			for(int i = 0; i <= max; i++) {
				cntString = "" + i;
				if(msg == cntString) return true;
			}

			return false;
		}

		// Clear the screen of the client
		public void clearClientScreen() {
			sOut.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
				           "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
				           "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" );
		}
	}
}

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
		private string msg, username, topic;
		private bool logged;

		public ClientManager(TcpClient clientSocket, Server server) {
			this.server = server;
			this.clientSocket = clientSocket;

			// Initialisation of the StreamWriter and the StreamReader
			sOut = new StreamWriter(clientSocket.GetStream());
			sOut.AutoFlush = true;
			sIn = new StreamReader(clientSocket.GetStream());

			logged = false;
			msg = "";
			username = "";
			topic = "";
		}

		public void run() {
			// Connection menu
			bool next = false;
			bool quit = false;
			while(!next) {
				ClearClientScreen();
				sOut.WriteLine("What do you want to do?\n" +
					"1: Log In\n"     +
					"2: Register\n\n" +
					"0: Quit\n\n"     +
					"Your choice: "   );
				msg = sIn.ReadLine();

				switch(msg) {
					case "0":
						ClearClientScreen();
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
				ClearClientScreen();
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
						ClearClientScreen();
						quit = true;
						break;
					case "1":
						JoinTopic();
						break;
					case "2":
						CreateTopic();
						break;
					case "3":
						SeeConnectedUsers();
						break;
					default:
						break;
				}
			}

			// send a message to close the client
			sOut.WriteLine("Server: bye");
			// remove this client manager from the list of the connected clients
			server.Logout(this);
		}

		// Ask the user for credentials and check if they are valid
		public bool Login() {
			ClearClientScreen();
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
							logged = true;
							return true;
						}
					}
				}
			}

			return false;
		}

		// Ask the user for credentials and add a new user in the list of the server
		public bool Register() {
			ClearClientScreen();
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
					this.username = username;
					logged = true;
					return true;
				}
			}

			return false;
		}

		// Display the list of the topics and let the user choose which one he wants to join
		public void JoinTopic() {
			bool next = false;
			int cnt, msgInt;

			while(!next) {
				ClearClientScreen();

				// display the list of the topics
				string strDisplay = "List of the topics\n--------------------\n\n";

				cnt = 1;
				foreach(string topic in server.Topics) {
					strDisplay += (cnt++) + ": " + topic + "\n";
				}

				strDisplay += "\n0: Cancel\n\n" +
					"Which topic do you want to join?\n" +
					"Your choice:";
				sOut.WriteLine(strDisplay);

				// wait for user's choice
				msg = sIn.ReadLine();
				if(MessageOk(msg, server.Topics.Count)) {
					next = true;
					// join the topic the user asked
					if(msg != "0") {
						msgInt = 0;
						try { msgInt = Int32.Parse(msg); }
						catch(FormatException e) { Console.WriteLine(e.Message); }
						topic = server.Topics[--msgInt];
						SendTopicMessages();
					}
				}
			}
		}

		// Ask the user for a name and add a new topic in the list of the server
		public void CreateTopic() {
			ClearClientScreen();

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
							ClearClientScreen();
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

		// Loop while the user wants to send messages in the current topic
		public void SendTopicMessages() {
			// display welcome message to all client
			// so everybody knows that a new client is in the topic
			ClearClientScreen();
			SendTopicMessage("Server: " + username + " has joined the " + topic + " topic", false);
			sOut.WriteLine("Server: Welcome on the " + topic + " topic " + username + "!\n" +
				"At any moment, type /q to quit this topic.");

			while(msg != "/q") {
				// wait for messages
				msg = sIn.ReadLine();

				if(msg != "/q") {
					// format the messages and send it
					msg = username + ": " + msg;
					SendTopicMessage(msg, false);
				}
			}

			// leave the topic
			SendTopicMessage("Server: " + username + " has left the topic", true);
			topic = "";
		}

		// Send a message to all user in the current topic
		public void SendTopicMessage(string msg, bool includeMe) {
			foreach(ClientManager client in server.ConnectedClients) {
				if(client.Topic == topic) {
					if(client != this)
						client.DisplayMessage(msg);
					else if(includeMe)
						client.DisplayMessage(msg);
				}
			}
		}

		// Method called by another client manager to display a message
		public void DisplayMessage(string msg) {
			sOut.WriteLine(msg);
		}


		// Display the list of the connected and logged users and let the user choose which one he wants to talk to
		public void SeeConnectedUsers() {
			ClientManager privateMessages;
			bool next = false;
			int cnt, msgInt;

			while(!next) {
				ClearClientScreen();

				List<ClientManager> loggedClients = new List<ClientManager>();

				// display the list of the connected and logged users
				string strDisplay = "List of the connected users\n-----------------------\n\n";

				cnt = 1;
				foreach(ClientManager client in server.ConnectedClients) {
					if(client.Logged && client != this) {
						loggedClients.Add(client);
						strDisplay += (cnt++) + ": " + client.Username + "\n";
					}
				}

				if(loggedClients.Count == 0)
					strDisplay += "No one else is connected :'(\n";

				strDisplay += "\n0: Cancel\n\n" +
					"Who do you want to send a message to?\n" +
					"Your choice:";
				sOut.WriteLine(strDisplay);

				// wait for user's choice
				msg = sIn.ReadLine();
				if(MessageOk(msg, loggedClients.Count)) {
					next = true;
					// join the topic the user asked
					if(msg != "0") {
						msgInt = 0;
						try { msgInt = Int32.Parse(msg); }
						catch(FormatException e) { Console.WriteLine(e.Message); }
						privateMessages = loggedClients[--msgInt];
						SendPrivateMessages(privateMessages);
					}
				}
			}
		}

		public void SendPrivateMessages(ClientManager privateMessages) {
			ClearClientScreen();
			sOut.WriteLine("Server: You are chatting with " + privateMessages.Username + ".\n" +
				"At any moment, type /q to quit this private chat");

			msg = sIn.ReadLine();
			while(msg != "/q") {
				// format the message and send it
				msg = username + " (private): " + msg;
				privateMessages.DisplayMessage(msg);

				// wait for messages
				msg = sIn.ReadLine();
			}

			// leave the discussion
			//privateMessages.DisplayMessage("Server: " + username + " is away");
		}

		// Check if the message is a number between 0 and the max value
		private bool MessageOk(string msg, int max) {
			string cntString;

			for(int i = 0; i <= max; i++) {
				cntString = "" + i;
				if(msg == cntString) return true;
			}

			return false;
		}

		// Clear the screen of the client
		public void ClearClientScreen() {
			sOut.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
				           "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" +
				           "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" );
		}

		// getters and setters
		public string Username {
			get { return username; }
		}

		public string Topic {
			get { return topic; }
		}

		public bool Logged {
			get { return logged; }
		}
	}
}

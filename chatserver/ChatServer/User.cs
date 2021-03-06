﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer {
	[Serializable]
	class User {
		private string _username;
		private string _password;

		public User(string username, string password) {
			_username = username;
			_password = password;
		}

		// getters and setters
		public string Username {
			get { return _username; }
		}

		public string Password {
			get { return _password; }
		}
	}
}

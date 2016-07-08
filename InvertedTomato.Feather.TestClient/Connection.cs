using System;
using InvertedTomato;
using InvertedTomato.Feather;

namespace InvertedTomato.Feather.TestClient {
    class Connection : ConnectionBase {

        public void GenerateAuthenticationKey(string emailAddress, string password) {
            if (null == emailAddress) {
                throw new ArgumentNullException("emailAddress");
            }
            if (null == password) {
                throw new ArgumentNullException("password");
            }

            Send(new Payload(0x00, emailAddress.GetBytes(), password.GetBytes()));
        }
        
        protected override void OnMessageReceived(Payload payload) {

        }
    }
}

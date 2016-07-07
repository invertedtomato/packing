using System;
using System.IO;
using InvertedTomato;

namespace InvertedTomato.Feather.TestClient {
    class Connection : ConnectionBase {

        public void GenerateAuthenticationKey(string emailAddress, string password) {
            if (null == emailAddress) {
                throw new ArgumentNullException("emailAddress");
            }
            if (null == password) {
                throw new ArgumentNullException("password");
            }

            using(var stream = new MemoryStream()) {
                stream.Write((byte)0x00);
                stream.Write(emailAddress);
                stream.Write(password);

                Send(stream.ToArray());
            }
        }
        
        protected override void OnMessageReceived(byte[] payload) {

        }
    }
}

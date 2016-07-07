using System;
using System.Net;

namespace InvertedTomato {
    public static class IPEndPointUtility {
        public static IPEndPoint Parse(string text, int defaultPort = 0) {
            Uri uri;
            IPAddress ipAddress;
            if ((!text.Contains(":") || text.Contains("://")) && Uri.TryCreate(text, UriKind.Absolute, out uri)) {
                if (!IPAddress.TryParse(uri.Host, out ipAddress)) {
                    ipAddress = getIPfromHost(uri.Host);
                }
                return new IPEndPoint(ipAddress, uri.Port < 0 ? defaultPort : uri.Port);
            }
            if (Uri.TryCreate(String.Concat("tcp://", text), UriKind.Absolute, out uri)) {
                if (!IPAddress.TryParse(uri.Host, out ipAddress)) {
                    ipAddress = getIPfromHost(uri.Host);
                }
                return new IPEndPoint(ipAddress, uri.Port < 0 ? defaultPort : uri.Port);
            }
            if (Uri.TryCreate(String.Concat("tcp://", String.Concat("[", text, "]")), UriKind.Absolute, out uri))
                return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port < 0 ? defaultPort : uri.Port);

            throw new FormatException("Failed to parse text to IPEndPoint");
        }

        private static IPAddress getIPfromHost(string p) {
            var hosts = Dns.GetHostAddresses(p);

            if (hosts == null || hosts.Length == 0)
                throw new ArgumentException(string.Format("Host not found: {0}", p));

            return hosts[0];
        }
    }
}
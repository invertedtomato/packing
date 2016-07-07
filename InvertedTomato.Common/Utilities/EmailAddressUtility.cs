using System;

namespace InvertedTomato {
    public static class EmailAddressUtility {
        public static bool IsValidEmailAddress(string emailAddress) {
            if (string.IsNullOrEmpty(emailAddress)) {
                return false;
            }

            try {
                var addr = new System.Net.Mail.MailAddress(emailAddress);
                return addr.Address == emailAddress;
            } catch (FormatException) {
                return false;
            }
        }
    }
}
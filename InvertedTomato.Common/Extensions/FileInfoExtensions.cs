using System;
using System.IO;
using Microsoft.Win32;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace InvertedTomato {
	public static class FileInfoExtensions {
		/// <summary>
		/// Get the contents of the file.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static string GetFileContents(this FileSystemInfo target) {
			Contract.Requires(target != null, "v");
			Contract.Requires(target.FullName.Length > 0);

			return File.ReadAllText(target.FullName);
		}

		/// <summary>
		/// Replace the file with given text.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="content"></param>
		public static void PutFileContents(this FileSystemInfo target, string content) {
			Contract.Requires(target != null, "v");
			Contract.Requires(content != null, "emailBody");

			File.WriteAllText(target.FullName, content);
		}

		/// <summary>
		/// Try and determine the content type of the file.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		public static string GetContentType(this FileSystemInfo target) {
			Contract.Requires(target != null, "v");

			// http://stackoverflow.com/questions/58510/using-net-how-can-you-find-the-mime-type-of-a-file-based-on-the-file-signature
			var contentType = "application/unknown";

			// Open key
			var regKey = Registry.ClassesRoot.OpenSubKey(target.Extension.ToUpperInvariant());
			if (null != regKey) {

				// Open v
				var v = regKey.GetValue("Content Type");
				if (null != v) {
					contentType = v.ToString();
				}
			}

			return contentType;
		}
	}
}

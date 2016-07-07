using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace InvertedTomato {
	public static class EnumExtensions {
		/// <summary>
		/// Returns Description Attribute information for an enum value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[ContractVerification(false)] // I can't seem to suppress code contracts in any better way
		public static string ToDescription(this Enum value) {
			Contract.Requires(null != value);

			// Use reflection to lookup value
			var fi = value.GetType().GetField(value.ToString());
			if (null == fi) {
				return value.ToString(); // The value isn't in the enum
			}
			var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

			// If description isn't found, fall-back to the label
			if (null == attributes || attributes.Length == 0 || attributes[0] == null) {
				return value.ToString();
			}

			// Return description
			return attributes[0].Description;
		}
	}
}

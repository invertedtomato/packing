using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace InvertedTomato {
    public static class IEnumerableExtensions {
		/*
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> value, Func<TSource, TKey> keySelector) {
            Contract.Requires(null != value);
            Contract.Requires(null != keySelector);

            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in value) {
                if (seenKeys.Add(keySelector(element))) {
                    yield return element;
                }
            }
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> value, string tableName) {
            Contract.Requires(null != value);
            Contract.Requires(null != tableName);

            var tbl = ToDataTable(value);
            tbl.TableName = tableName;
            return tbl;
        }

        /// <summary>
        /// Convert the Enumerable into an SQL Data Table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> value) {
            Contract.Requires(null != value);

            var properties = typeof(T).GetProperties();

            var table = new DataTable();
            var cols = new HashSet<string>();

            // Build the columns
            foreach (var prop in properties) {
                var type = prop.PropertyType;

                // Resolve nullable
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    type = Nullable.GetUnderlyingType(type);
                }

                // Resolve enum
                if (prop.PropertyType.IsEnum) {
                    type = Enum.GetUnderlyingType(prop.PropertyType);
                }

                // Check type of type :)
                if (type == typeof(string) ||
                    type == typeof(long) ||
                    type == typeof(int) ||
                    type == typeof(byte) ||
                    type == typeof(Guid) ||
                    type == typeof(bool) ||
                    type == typeof(decimal) ||
                    type == typeof(float) ||
                    type == typeof(double) ||
                    type == typeof(DateTime)) {
                    table.Columns.Add(prop.Name, type);
                    cols.Add(prop.Name.ToString());
                }
            }

            // Fill the DataTable
            foreach (var item in value) {
                var row = table.NewRow();

                foreach (var prop in properties) {
                    // Skip excluded cols
                    if (!cols.Contains(prop.Name)) {
                        continue;
                    }

                    var itemValue = prop.GetValue(item, null);
                    row[prop.Name] = itemValue;
                }

                table.Rows.Add(row);
            }

            return table;
        }
		*/


        /// <summary>
        /// Convert Enumerable to HashSet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> target) {
            Contract.Requires(null != target);

            return new HashSet<T>(target);
        }

		/// <summary>
		/// Fire action on each item in enumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <param name="action"></param>
		public static void Each<T>(this IEnumerable<T> target, Action<T> action) {
			Contract.Requires(null != target);
			Contract.Requires(null != action);

			foreach (var item in target) {
				action(item);
			}
		}
    }
}

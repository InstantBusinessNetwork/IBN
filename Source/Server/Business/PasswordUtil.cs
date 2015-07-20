using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Mediachase.IBN.Business
{
	internal sealed class PasswordUtil
	{
		public const int SaltSize = 5;

		#region CreateSalt
		/// <summary>
		/// Creates the salt.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <returns></returns>
		internal static string CreateSalt(int size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException("size");

			// Generate a cryptographic random number using the cryptographic
			// service provider
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			byte[] buff = new byte[size];
			rng.GetBytes(buff);

			// Return a Base64 string representation of the random number
			return Convert.ToBase64String(buff);
		}
		#endregion

		#region CreateHash
		/// <summary>
		/// Creates the hash.
		/// </summary>
		/// <param name="password">The password.</param>
		/// <param name="salt">The salt.</param>
		/// <returns></returns>
		internal static string CreateHash(string password, string salt)
		{
			if (password == null)
				throw new ArgumentNullException("password");
			if (salt == null)
				throw new ArgumentNullException("salt");

			// Concat the raw password and salt value
			string saltAndPwd = String.Concat(password, "$", salt);

			// Hash the salted password
			string hashedPwd = HashPassword(saltAndPwd, "md5");

			return hashedPwd;
		}
		#endregion

		#region HashPassword
		/// <summary>
		/// Hashes the password for storing in config file.
		/// </summary>
		/// <param name="password">The password.</param>
		/// <param name="passwordFormat">The password format.</param>
		/// <returns></returns>
		private static string HashPassword(string password, string passwordFormat)
		{
			if (password == null)
				throw new ArgumentNullException("password");
			if (passwordFormat == null)
				throw new ArgumentNullException("passwordFormat");

			passwordFormat = passwordFormat.ToLower();

			HashAlgorithm algorithm;

			switch (passwordFormat)
			{
				case "sha1":
					algorithm = SHA1.Create();
					break;
				case "md5":
					algorithm = MD5.Create();
					break;
				default:
					throw new ArgumentException("passwordFormat");
			}

			return Convert.ToBase64String(algorithm.ComputeHash(Encoding.Unicode.GetBytes(password)));
		}
		#endregion

		#region Check
		/// <summary>
		/// Checks the specified password.
		/// </summary>
		/// <param name="password">The password.</param>
		/// <param name="salt">The salt.</param>
		/// <param name="hash">The hash.</param>
		/// <returns></returns>
		internal static bool Check(string password, string salt, string hash)
		{
			if (password == null)
				throw new ArgumentNullException("password");
			if (salt == null)
				throw new ArgumentNullException("salt");
			if (hash == null)
				throw new ArgumentNullException("hash");

			string newHash = CreateHash(password, salt);

			return newHash == hash;
		}
		#endregion
	}
}

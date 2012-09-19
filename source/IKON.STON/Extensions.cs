using System;
using IKON.STON.Values;

namespace IKON.STON
{
	/// <summary>
	/// Various extension methods that adds IKSTON specific functionality to IKON classes.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Casts base IKON value to IKSTON array
		/// </summary>
		/// <param name="value"></param>
		/// <returns>IKSTON array</returns>
		public static IKON.STON.Values.Array AsArray(this Value value)
		{
			try
			{
				return value as IKON.STON.Values.Array;
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Casts base IKON value to IKSTON number
		/// </summary>
		/// <param name="value"></param>
		/// <returns>IKSTON number</returns>
		public static Numeric AsNumber(this Value value)
		{
			try
			{
				return value as Numeric;
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Casts base IKON value to IKSTON object (composite value)
		/// </summary>
		/// <param name="value"></param>
		/// <returns>IKSTON object</returns>
		public static IKON.STON.Values.Object AsObject(this Value value)
		{
			try
			{
				return value as IKON.STON.Values.Object;
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

		/// <summary>
		/// Casts base IKON value to IKSTON text
		/// </summary>
		/// <param name="value"></param>
		/// <returns>IKSTON text</returns>
		public static Text AsText(this Value value)
		{
			try
			{
				return value as Text;
			}
			catch (InvalidCastException e)
			{
				throw e;
			}
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64
{
    public static class Base64
    {
        private static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		// Comme dans la librairie de base, le caractere special ajoute a la fin des bytes imcomplets est '=' 
		private static readonly char Special = '='; 

		/// <summary>
		/// Elle encode un tableau d'octet en Base64 
		/// </summary>
		/// <param name="source">Le tableau d'octets (byte) a encoder</param>
		/// <param name="parallel">Mettez 'true' si vous desirez utiliser toute la puissance de calcul de votre processeur</param>
		/// <returns></returns>
		public static string Encode(byte[] source, bool parallel = false)
		{
			if (source == null || source.Length == 0)
				return "";

			// Ici, je determine le nombre de caracteres principaux du resultat final en tenant
			// compte que les caractere sont sur 6 bits alors que les bytes sont sur 8 bits :
			// Le nombre total de bits dans source est de source.Length * 8. Maintenant il faut diviser par 24 pour 
			// avoir le nombre de 24 bits il y a dedans. Ensuite on sait que dans un 24 bits il y a 4 * 6 bits dedans
			// Enfin faisant source.Length * 8/24 * 4 on obtient source.Length * 1/3 * 4
			// Le cas ou source.Length == 1 nous amene a la formule (source.Length + 2) / 3 * 4
			int resultLength = (source.Length + 2) / 3 * 4;
			char[] result = new char[resultLength];

			// Encodage des 3 * n premiers bytes avec n == (nombre total de byte) division entiere par 3

			int length3 = source.Length / 3;
			if (!parallel)
			{
				EncodeBlock(source, result, 0, length3);
			}
			else
			{
				int processorCount = Math.Min(length3, Environment.ProcessorCount);
				System.Threading.Tasks.Parallel.For(0, processorCount, i =>
				{
					int beginInd = i * length3 / processorCount;
					int endInd = (i + 1) * length3 / processorCount;
					EncodeBlock(source, result, beginInd, endInd);
				});
			}


			// Encodage des n derniers bytes avec n == (nombre total de byte) % 3

			int index;
			int x1, x2;
			int srcIndex, dstIndex;
			switch (source.Length - length3 * 3)
			{
				case 1:
					index = length3;
					srcIndex = index * 3;
					dstIndex = index * 4;
					x1 = source[srcIndex];
					result[dstIndex] = Alphabet[x1 >> 2];
					result[dstIndex + 1] = Alphabet[(x1 << 4) & 0x30];
					// Je complete le reste par le caractere special
					result[dstIndex + 2] = Special;
					result[dstIndex + 3] = Special;
					break;
				case 2:
					index = length3;
					srcIndex = index * 3;
					dstIndex = index * 4;
					x1 = source[srcIndex];
					x2 = source[srcIndex + 1];
					result[dstIndex] = Alphabet[x1 >> 2];
					result[dstIndex + 1] = Alphabet[((x1 << 4) & 0x30) | (x2 >> 4)];
					result[dstIndex + 2] = Alphabet[(x2 << 2) & 0x3C];
					// Je complete le reste par le caractere special
					result[dstIndex + 3] = Special;
					break;
			}

			return new string(result);
		}

		private static void EncodeBlock(byte[] src, char[] dst, int beginIndex, int endIndex)
		{
			for (int index = beginIndex; index < endIndex; index++)
			{
				int srcIndex = index * 3;
				int dstIndex = index * 4;

				byte x1 = src[srcIndex];
				byte x2 = src[srcIndex + 1];
				byte x3 = src[srcIndex + 2];

				// Ces operations de decalage de bits servent juste a
				// selectionner les bits importants formant la position correspondante dans le
				// la string Alphabet
				dst[dstIndex] = Alphabet[x1 >> 2];
				dst[dstIndex + 1] = Alphabet[((x1 << 4) & 0x30) | (x2 >> 4)];
				dst[dstIndex + 2] = Alphabet[((x2 << 2) & 0x3C) | (x3 >> 6)];
				dst[dstIndex + 3] = Alphabet[x3 & 0x3F];
			}
		}
	}
}

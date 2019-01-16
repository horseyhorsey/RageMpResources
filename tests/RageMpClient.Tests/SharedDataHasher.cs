using NeoSmart.Hashing.XXHash;
using System.IO;
using Xunit;

namespace RageMpClient.Tests
{
    public class SharedDataHasher
    {
        //Add all shared variables here used in your game
        string[] hashes = new string[] { "PLY_LEVEL", "PLY_CASH"};

        /// <summary>
        /// Where to save the hashes with key
        /// </summary>
        const string EXPORT_PATH = @"..\..\..\SharedDataHashes.txt";

        /// <summary>
        /// Generate hashes with XXHASh64 from the hashes array and prints to EXPORT_PATH
        /// </summary>
        [Fact]
        public void CompHas()
        {                        
            using (var sw = File.CreateText(EXPORT_PATH))
            {
                foreach (var key in hashes)
                {
                    sw.WriteLine($"{key}\t\t = {XXHash64.Hash(0, key)}");
                }
            }

            Assert.True(File.Exists(EXPORT_PATH));
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base64;

namespace EncoderTesteur
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 0x6A, 0x77, 0xC4
             * 0x6A, 0x77, 0xC4, 0x31, 0x45, 0x52 
             * 
             */

            byte[] mySource = { 0x6A, 0x77, 0xC4, 0x31, 0x45, 0x6A, 0x77, 0xC4, 0x31, 0x45, 0x6A, 0x77, 0xC4, 0x31, 0x45, 0x6A, 0x77, 0xC4, 0x31, 0x45, 0x6A, 0x77, 0xC4, 0x31, 0x45, 0x6A, 0x77, 0xC4, 0x31, 0x45 };
            string other = System.Convert.ToBase64String(mySource);
            string result = Base64.Base64.Encode(mySource, true);            
        }
    }
}

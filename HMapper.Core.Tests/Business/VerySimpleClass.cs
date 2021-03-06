﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMapper.Tests.Business
{
    public class VerySimpleClass
    {
        public string MyString;
        public int MyInt;
        static Random rnd = new Random();

        public static VerySimpleClass Create()
        {
            return new VerySimpleClass()
            {
                MyString = $"str{rnd.Next()}",
                MyInt = rnd.Next()
            };
        }

        public static VerySimpleClass[] CreateMany(int nb)
        {
            return Enumerable.Range(1, nb).Select(i => Create()).ToArray();
        }
    }
}

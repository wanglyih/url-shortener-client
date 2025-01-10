using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_client
{
    internal sealed class SequentialGuid
    {
        private static SequentialGuid sequentialGuid = null;
        private static readonly object lockObj = new object();

        private SequentialGuidValueGenerator generator;
        SequentialGuid() { 
            this.generator = new SequentialGuidValueGenerator();
        }

        public static SequentialGuid Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (sequentialGuid == null)
                    {
                        sequentialGuid = new SequentialGuid();
                    }
                    return sequentialGuid;
                }
            }
        }

        public string Get()
        {           
            var guid = this.generator.Next(null);
            var id = guid.ToString().Replace("-", "");
            var url = $"https://google.com//search?q={id}";

            return url;
        } 
    }
}

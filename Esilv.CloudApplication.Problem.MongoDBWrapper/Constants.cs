using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESILV.CloudApplication.Problem.MongoDBWrapper
{
    public static class Constants
    {
        public static readonly string PORT = "27017";
        public static readonly string CONNECTION_STRING = "mongodb://localhost" + ":" + PORT;
        public static readonly string DATABASE_NAME = "test";
        public static readonly string COLLECTION_NAME = "test";

    }
}

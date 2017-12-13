using MongoDB.Driver.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESILV.CloudApplication.Problem.MongoDBWrapper
{
    public class Wrapper
    {
        private IConnection _connection;
        private CancellationToken _cancellationToken;
        private CancellationTokenSource _source;

        public Wrapper()
        {

        }

        public void Connect()
        {
            this._source = new CancellationTokenSource();
            this._connection.Open(_source.Token);
        }

        public void Disconnect()
        {
            this._source.Cancel();
        }

    }
}

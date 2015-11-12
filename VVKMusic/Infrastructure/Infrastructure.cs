using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Status = Common.Common.Status;

namespace Infrastructure
{
    public class Infrastructure
    {
        public Status CheckConnection()
        {
            return Status.OK;
        }
        public Status SaveListOfUsers()
        {
            return Status.OK;
        }
        public Status LoadListOfUsers()
        {
            return Status.OK;
        }
    }
}

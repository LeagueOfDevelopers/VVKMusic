using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Status = Common.Common.Status;

namespace Infrastructure
{
    public class Infrastructure
    {
        public Status CheckConnection()
        {
            return Status.OK;
        }
        public Status SaveListOfUsers(List<User> listOfUsers)
        {
            FileStream fs = new FileStream("Users.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, listOfUsers);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            return Status.OK;
        }
        public List<User> LoadListOfUsers()
        {
            FileStream fs = new FileStream("Users.dat", FileMode.OpenOrCreate);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (List<User>)formatter.Deserialize(fs);
            }
            catch (SerializationException) { return null; }
            finally
            {
                fs.Close();
            }
        }
    }
}

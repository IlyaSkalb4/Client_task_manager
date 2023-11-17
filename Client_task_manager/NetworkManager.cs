using Classes_for_transferring_users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client_task_manager
{
    internal class NetworkManager
    {
        private TcpClient tcpClient = null;

        private List<UserTask> userTasks = null;

        private ReadyPackage receivedPackage = null;

        private NetworkStream networkStream = null;

        private IFormatter formatter = null;

        private bool flagRequestApproved;

        public List<UserTask> UserTasks { get {  return userTasks; } }

        public string ErrorType { get; private set; }

        public string ErrorMessage { get; private set; }

        public void ClearUserTasks()
        {
            if (userTasks != null)
            {
                userTasks.Clear();
            }
        }

        public async Task<bool> SendAndReceivePackageAsync(ReadyPackage readyPackage)
        {
            bool taskRunResult = false;

            flagRequestApproved = true;

            await Task.Run(() =>
            {
                taskRunResult = ConnectAndGetStream();
            });

            if (taskRunResult)
            {
                formatter = new BinaryFormatter();

                await Task.Run(() =>
                {
                    taskRunResult = SendPackage(readyPackage);
                });

                if (taskRunResult)
                {
                    await Task.Run(ReceivePaсkage);
                }
            }

            if (networkStream != null)
            {
                networkStream.Close();
            }

            if (tcpClient != null)
            {
                tcpClient.Close();
            }

            return flagRequestApproved;
        }

        private bool SendPackage(ReadyPackage readyPackage)
        {
            try
            {
                formatter.Serialize(networkStream, readyPackage);
            }
            catch (Exception ex)
            {
                CommonMethods.ShowErrorMessage(ex.Message);
                return false;
            }

            return true;
        }

        private bool ConnectAndGetStream()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(Constants.ServerIP, Constants.Port1024);

                networkStream = tcpClient.GetStream();
            }
            catch (Exception ex)
            {
                CommonMethods.ShowErrorMessage(ex.Message);

                return false;
            }

            return true;
        }

        private void ReceivePaсkage()
        {
            try
            {
                receivedPackage = (ReadyPackage)formatter.Deserialize(networkStream);

                string objType = receivedPackage.ObjType;
                object data = receivedPackage.Data;

                if (objType != null)
                {
                    if (objType == Constants.UserTask)
                    {
                        userTasks = (List<UserTask>)receivedPackage.Data;
                    }
                    else if(objType == Constants.Registration)
                    {
                        ErrorMessage = data.ToString();
                    }
                    else
                    {
                        flagRequestApproved = false;

                        ErrorType = objType;
                        if (data != null)
                        {
                            ErrorMessage = receivedPackage.Data.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonMethods.ShowErrorMessage(ex.Message);
            }
        }
    }
}

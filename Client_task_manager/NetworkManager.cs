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
            userTasks.Clear();
        }

        public async Task<bool> SendAndReceivePackageAsync(ReadyPackage readyPackage)
        {
            bool taskRunResult = false;

            flagRequestApproved = false;

            await Task.Run(() =>
            {
                taskRunResult = ConnectAndGetStream();
            });

            if (taskRunResult)
            {
                formatter = new BinaryFormatter();

                ErrorType = "";

                await Task.Run(() =>
                {
                    taskRunResult = SendPackage(readyPackage);
                });

                await Task.Run(() =>
                {
                    taskRunResult = ReceivePaсkage();
                });

                flagRequestApproved = taskRunResult;

                while (taskRunResult)
                {
                    await Task.Run(() =>
                    {
                        taskRunResult = ReceivePaсkage();
                    });
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

        private bool ReceivePaсkage()
        {
            try
            {
                receivedPackage = (ReadyPackage)formatter.Deserialize(networkStream);

                string objType = receivedPackage.ObjType;

                if (objType != "")
                {
                    if (objType == Constants.UserTask)
                    {
                        if (userTasks == null)
                        {
                            userTasks = new List<UserTask>();
                        }

                        if (objType != null)
                        {
                            userTasks.Add((UserTask)receivedPackage.Data);
                        }
                    }
                    else
                    {
                        ErrorType = objType;

                        ErrorMessage = receivedPackage.Data.ToString();
                    }
                }

                return receivedPackage.RepeatStatus;
            }
            catch (Exception ex)
            {
                CommonMethods.ShowErrorMessage(ex.Message);
            }

            return false;
        }
    }
}

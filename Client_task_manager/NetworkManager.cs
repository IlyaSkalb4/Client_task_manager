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
    internal class NetworkManager //Клас дає змогу надсилати запити серверу на реєстрацію, вхід у систему та отримання завдань.
    {
        private TcpClient tcpClient = null;

        private List<UserTask> userTasks = null;

        private ReadyPackage receivedPackage = null;

        private NetworkStream networkStream = null;

        private IFormatter formatter = null;

        private bool flagRequestApproved;

        public List<UserTask> UserTasks //Властивість, яка дає доступ до списку завдань користувача.
        {
            get
            { 
                return userTasks;
            }
        }

        public string ErrorType //Властивість, що зберігає тип помилки.
        {
            get;
            private set;
        } 

        public string ErrorMessage //Властивість, що зберігає саму помилку.
        {
            get;
            private set;
        }

        public void ClearUserTasks() //Метод, який очищає завдання для даного класу.
        {
            if (userTasks != null)
            {
                userTasks.Clear();
            }
        }

        public async Task<bool> SendAndReceivePackageAsync(ReadyPackage readyPackage) //Метод, у якому проходить повний алгоритм приймання-відправлення запитів і результатів.
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

        private bool SendPackage(ReadyPackage readyPackage) //Метод, який надсилає запити.
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

        private bool ConnectAndGetStream() //Метод, який реалізує підключення до сервера та отримання мережевого потоку.
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(Constants.ServerIP, Constants.Port);

                networkStream = tcpClient.GetStream();
            }
            catch (Exception ex)
            {
                CommonMethods.ShowErrorMessage(ex.Message);

                return false;
            }

            return true;
        }

        private void ReceivePaсkage() //Метод, який отримує та обробляє відповіді на запити.
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

using MSMQ.Messaging;
using MSMQ_Publisher_Process.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Configuration;
using System.Windows;

namespace MSMQ_Publisher_Process.ViewModel
{
    /// <summary>
    /// WellDataViewModel handles the logic between the model and view to Publish Data.
    /// The class uses MSMQ Messaging Nuget Package and BindableBase class from Prism Framework.
    /// </summary>
    public class WellDataViewModel : BindableBase
    {
        public WellDataViewModel()
        {
            machineName = Environment.MachineName;
            SendWellDataCommand = new DelegateCommand(SendWellDataCommandAction);
        }
        public DelegateCommand SendWellDataCommand { get; set; }
        private readonly string machineName;
        readonly string publicQueuePath = ConfigurationManager.AppSettings["PublicQueuePath"] ?? "default value";
        readonly string privateQueuePath = ConfigurationManager.AppSettings["PrivateQueuePath"] ?? "default value";
        private string GetMachinePublicQueuePath() => $"{machineName}{publicQueuePath}";
       
        /// <summary>
        /// This method calls the CreateQueue and SendDataToQueue methods to send well data  
        /// </summary>
        public void SendWellDataCommandAction()
        {
            CreateQueue();
            SendDataToQueue();
        }
        /// <summary>
        /// This method creates a queue if one does nor exist depending on the decision of the tenery operator using the genericQueuePath
        /// </summary>
        public void CreateQueue()
        {
            try
            {
                string genericQueuePath = (MessageQueue.Exists(GetMachinePublicQueuePath())) ? GetMachinePublicQueuePath() : privateQueuePath;
                if (!MessageQueue.Exists(genericQueuePath))
                {
                    MessageQueue.Create(genericQueuePath);
                }
            }
            catch (MessageQueueException ex)
            {
                MessageBox.Show("An error occured while creating the queue:" + ex.Message);
            }
        }
        /// <summary>
        /// This method connects to queue and sends data( well data) to the queue created depending on if the machine uses a public or private queue 
        /// </summary>
        public void SendDataToQueue()
        {
            try
            {
                if (MessageQueue.Exists(GetMachinePublicQueuePath()))
                {
                    string publicQueuePath = GetMachinePublicQueuePath();
                    MessageQueue queue = new(publicQueuePath);
                    WellDataModel wellData = MapWellDataProperties();
                    queue.Send(wellData, "CypherCrescentResource");
                    FieldName = string.Empty;
                    WellName = string.Empty;
                    DrainagePoint = string.Empty;
                }
                else
                {
                    MessageQueue queue = new(privateQueuePath);
                    WellDataModel wellData = MapWellDataProperties();
                    queue.Send(wellData, "CypherCrescentResource");
                    FieldName = string.Empty;
                    WellName = string.Empty;
                    DrainagePoint = string.Empty;
                }
            }
            catch (MessageQueueException ex)
            {
                MessageBox.Show("An error occured while sending data to the queue:" + ex.Message);
            }
        }
        private WellDataModel MapWellDataProperties()
        {
            return new()
            {
                WellName = WellName,
                FieldName = FieldName,
                DrainagePoint = DrainagePoint
            };
        }
        /// <summary>
        /// Backing fields and corresponding well data properties 
        /// </summary>
        private string fieldName;
        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; RaisePropertyChanged(); }
        }
        private string wellName;
        public string WellName
        {
            get { return wellName; }
            set { wellName = value; RaisePropertyChanged(); }
        }
        private string drainagePoint;
        public string DrainagePoint
        {
            get { return drainagePoint; }
            set { drainagePoint = value; RaisePropertyChanged(); }
        }
    }
}

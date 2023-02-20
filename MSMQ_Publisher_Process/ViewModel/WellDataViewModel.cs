﻿using MSMQ.Messaging;
using MSMQ_Publisher_Process.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        private readonly string machineName;
        readonly string queuePath = @"\publicmsmq";
        public DelegateCommand SendWellDataCommand { get; set; }
        /// <summary>
        /// This method calls the CreateQueue and SendDataToQueue methods to send well data  
        /// </summary>
        public void SendWellDataCommandAction()
        {
            CreateQueue();
            SendDataToQueue();
        }
        private string GetMachinePublicQueuePath() => $"{machineName}{queuePath}";
        /// <summary>
        /// This method specifies queuePath and creates a queue if one does nor exist
        /// </summary>
        //readonly string publicQueuePath = "FormatName:DIRECT=OS:CCLNG-PC5188.svr.cyphercrescent.com\\publicmsmq";
        //readonly string privatequeuePath = @".\private$\MSMQ_MessagingApp";
        //readonly string publicQueuePath = @"CCLNG-PC5188\publicmsmq";
        public void CreateQueue()
        {
            if (!MessageQueue.Exists(GetMachinePublicQueuePath()))
            {
                MessageQueue.Create(GetMachinePublicQueuePath());
            }
        }
        /// <summary>
        /// This method connects to queue and sends data( well data) to the queue created  
        /// </summary>
        public void SendDataToQueue()
        {
            string publicQueuePath = GetMachinePublicQueuePath();
            MessageQueue queue = new(publicQueuePath);
            WellDataModel wellData = MapWellDataProperties();
            queue.Send(wellData, "CypherCrescentResource");
            FieldName = string.Empty;
            WellName = string.Empty;
            DrainagePoint = string.Empty;
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
